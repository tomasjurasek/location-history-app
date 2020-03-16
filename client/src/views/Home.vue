<template>
    <div>
        <v-container class="header">
            <v-row align="center" justify="center" no-gutters>
                <v-col md="10">
                    <v-alert type="warning" v-if="!$route.params.id">
                        <strong>Chybí ID uživatele!</strong> <br />
                        Prosím, použijte odkaz, který vám přišel do mailu.
                    </v-alert>
                    <v-alert type="warning" v-if="uploadFailed">
                        <strong>Něco se pokazilo!</strong> <br />
                        Prosím, zkuste to znovu.
                    </v-alert>
                    <h2 class="header__title">Pomozte zjistit historii vaší polohy</h2>
                    <p class="short-instructions">
                        Historii polohy stáhněte z Google podle
                        <router-link
                            :to="{ name: 'Instructions' }"
                            color="primary"
                        >návodu</router-link>.<br />
                        Výsledný soubor je nazvaný např.
                        <strong>takeout-20200315T062605Z-001.zip</strong>, ten nahrajte zde:
                    </p>
                        <v-file-input
                            class="file-input"
                            placeholder="Vybrat soubor"
                            v-model="file"
                            background-color="white"
                            hide-details
                            outlined
                        ></v-file-input>
                        <v-btn
                            class="upload-button"
                            v-on:click="submitFile()"
                            :disabled="false && !file"
                            :loading="loading"
                            color="success"
                            x-large
                            elevation="0"
                            large
                        >Nahrát</v-btn>
                </v-col>
            </v-row>
        </v-container>
        <v-container>
            <v-row align="center" justify="center">
                <Instructions />
            </v-row>

            <div class="mt-8">
                <v-btn :to="{ name: 'LocationHistory' }">
                    Mapa
                </v-btn>
            </div>
        </v-container>
    </div>
</template>

<style scoped>
    .header {
        padding-top: 48px;
        padding-bottom: 48px;

        background-color: #555555;
        color: white;

        text-align: center;
    }

    .header__title {
        font-size: 32px;
        font-weight: 500;
    }

    .short-instructions {
        padding: 24px;
        font-size: 14px;
        font-weight: 300;
    }

    .short-instructions a {
        color: inherit;
    }

    .file-input {
        max-width: 460px;
        margin: 0 auto !important;
    }

    .file-input /deep/ .v-input__prepend-outer {
        position: absolute;
        top: 0;
        left: 16px;
        margin-top: 16px;
        z-index: 100;
    }

    .file-input /deep/ .v-input__prepend-outer button {
        color: black;
    }

    .file-input /deep/ .v-input__slot {
        padding-left: 5em !important;
    }

    .upload-button {
        min-width: 180px !important;
        margin-top: 16px
    }
</style>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import axios from "axios";
import Instructions from "./Instructions.vue";

@Component({ components: { Instructions } })
export default class Home extends Vue {
    file: any = null;
    uploadFailed = false;
    loading = false;
    submitFile() {
        this.uploadFailed = false;
        this.loading = true;
        const formData = new FormData();
        formData.append("file", this.file);
        axios
            .post(
                `${process.env.VUE_APP_API_URL}/users/${this.$route.params.id}/file`,
                formData,
                {
                    headers: {
                        "Content-Type": "multipart/form-data"
                    }
                }
            )
            .then(() => {
                this.loading = false;
                console.log("Hotovo");
            })
            .catch(e => {
                this.loading = false;
                console.log("Chybka", e);
                this.uploadFailed = true;
            });
    }
}
</script>
