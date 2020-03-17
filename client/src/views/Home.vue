<template>
    <div>
        <v-container class="header">
            <v-row align="center" justify="center" no-gutters>
                <v-col md="10">
                    <v-alert type="warning" v-if="uploadFailed">
                        <strong>Něco se pokazilo!</strong> <br />
                        Prosím, zkuste to znovu.
                    </v-alert>
                    <h2 class="header__title">
                        Pomozte zjistit historii vaší polohy
                    </h2>
                    <p class="short-instructions">
                        Historii polohy stáhněte z Google podle návodu .<br />
                        Výsledný soubor (nazvaný např.
                        <strong>takeout-20200315T062605Z-001.zip</strong>)
                        nahrajte zde:
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
                        >Nahrát</v-btn
                    >
                </v-col>
            </v-row>
        </v-container>
        <v-container>
            <Instructions />

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
    margin-top: 16px;
}
</style>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import axios from "axios";
import Instructions from "./Instructions.vue";
import { locationHistoryStorage } from "@/services/LocationHistoryStorage";

@Component({ components: { Instructions } })
export default class Home extends Vue {
    file: any = null;
    uploadFailed = false;
    loading = false;
    id: string = this.generateId();


    generateId() {
        return this.rand3Digits() + '-' + this.rand3Digits() + '-' + this.rand3Digits();
    }

    rand3Digits() {
        return ("000" + Math.floor((Math.random() * 1000))).slice(-3);
    }

    submitFile() {
        this.uploadFailed = false;
        this.loading = true;
        const formData = new FormData();
        formData.append("file", this.file);
        axios.post(
                `${process.env.VUE_APP_API_URL}/users/${this.id}/file`,
                formData,
                {
                    headers: {
                        "Content-Type": "multipart/form-data"
                    }
                }
            )
            .then((response) => {
                this.loading = false;
                /*
                {
                    "id": "string",
                     "locations": [
                        {
                          "dateTimeUtc": "2020-03-16T19:39:46.393Z",
                          "longitude": 0,
                          "latitude": 0,
                          "accuracy": 0
                        }
                      ]
                 }
               */
                locationHistoryStorage.save(response.data);
                this.$router.push({ path: 'map' })
            })
            .catch(e => {
                this.loading = false;
                console.log("Chybka", e);
                //Use fake data in case of error until backend is ready
                const locationHistory = {
                    id: this.id,
                    locations: [
                        {
                            "dateTimeUtc": "20200316T14:00:00Z",
                            "latitude": 500437725,
                            "longitude": 144549068,
                            "accuracy": 96,
                        },
                        {
                            "dateTimeUtc": "20200316T15:00:00Z",
                            "latitude": 500437275,
                            "longitude": 144545330,
                            "accuracy": 33,
                        },
                    ]
                };
                locationHistoryStorage.save(locationHistory);
                this.$router.push({ path: 'map' })
            });
    }
}
</script>
