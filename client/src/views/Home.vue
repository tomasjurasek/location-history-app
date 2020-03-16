<template>
    <div class="pa-3">
        <v-container fill-height fluid>
            <v-row align="center" justify="center">
                <v-alert type="warning" v-if="!$route.params.id">
                    <strong>Chybí ID uživatele!</strong> <br />
                    Prosím, použijte odkaz, který vám přišel do mailu.
                </v-alert>
                <v-alert type="warning" v-if="uploadFailed">
                    <strong>Něco se pokazilo!</strong> <br />
                    Prosím, zkuste to znovu.
                </v-alert>
            </v-row>
            <v-row align="center" justify="center">
                <p>
                    Historii polohy stáhněte z Google podle
                    <router-link :to="{ name: 'Instructions' }" color="primary"
                        >návodu</router-link
                    >, výsledný soubor je nazvaný např.
                    <em>takeout-20200315T062605Z-001.zip</em>, ten nahrajte zde.
                </p>
            </v-row>
            <v-row align="center" justify="center">
                <v-form>
                    <v-file-input
                        show-size
                        counter
                        label="Soubor"
                        v-model="file"
                    ></v-file-input>
                    <v-btn
                        class="primary ma-2"
                        v-on:click="submitFile()"
                        :disabled="!file"
                        :loading="loading"
                        >Nahrát</v-btn
                    >
                    <!-- <v-btn class="ma-2" :to="{ name: 'Instructions' }"
                >Jak připravit data?</v-btn
              > -->
                </v-form>
            </v-row>
            <v-row align="center" justify="center">
                <Instructions />
            </v-row>
        </v-container>

        <div class="mt-8">
            <v-btn :to="{ name: 'LocationHistory' }">
                Mapa
            </v-btn>
        </div>
    </div>
</template>

<style scoped></style>

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
