<template>
    <v-col class="col-5">
        <v-row align="center" justify="center">
            <h1>Navod jak stahnut data z Google</h1>
        </v-row>

        <v-row align="center" justify="center">
            <v-col align="center" justify="center">
                <v-row align="center">
                    <v-col>
                        <h1>1.</h1>
                    </v-col>
                    <v-col>
                        <h2>Přihlaste se do Google Takeout</h2>
                        <p>
                            <a
                                href="https://takeout.google.com/u/0/settings/takeout?pli=1"
                                target="_blank"
                                >https://takeout.google.com/u/0/settings/takeout?pli=1</a
                            ><br />
                            Klikněte na <strong>Zrušit výběr všech</strong> a
                            vyberte pouze historii polohy.
                        </p>
                    </v-col>
                </v-row>
                <v-row align="center" justify="end">
                    <v-sheet color="white">
                        <v-img
                            src="instructions/instructions1.png"
                            max-width="500"
                            class="elevation-4"
                        />
                    </v-sheet>
                </v-row>
            </v-col>
        </v-row>

        <v-row align="center" justify="center">
            <v-col align="center" justify="center">
                <v-row align="center">
                    <v-col>
                        <h1>2.</h1>
                    </v-col>
                    <v-col>
                        <p>
                            Nechte formát dat JSON a na konci stránky klepněte
                            na
                            <strong>pokračovat</strong>.
                        </p>
                    </v-col>
                </v-row>
                <v-row align="center" justify="end">
                    <v-sheet class="pa-12" color="white">
                        <v-img
                            src="instructions/instructions2.png"
                            max-width="500"
                            class="elevation-4"
                        />
                        <br />
                    </v-sheet>
                </v-row>
            </v-col>
        </v-row>

        <v-row align="center" justify="center">
            <v-col align="center" justify="center">
                <v-row align="center">
                    <v-col>
                        <h1>3.</h1>
                    </v-col>
                    <v-col>
                        <p>
                            Nechte si odkaz zaslat e-mailem, zvolte exportovat
                            jednou a klikněte na
                            <strong>Vytvořit export</strong>. Objeví se
                            následující obrazovka s informací, že dochází ke
                            stahování.
                        </p>
                    </v-col>
                </v-row>
                <v-row align="center" justify="end">
                    <v-sheet class="pa-12" color="white">
                        <v-img
                            src="instructions/instructions3.png"
                            max-width="500"
                            class="elevation-4"
                        />
                    </v-sheet>
                </v-row>
            </v-col>
        </v-row>

        <v-row align="center" justify="center">
            <v-col align="center" justify="center">
                <v-row align="center">
                    <v-col>
                        <h1>4.</h1>
                    </v-col>
                    <v-col>
                        <h2>Čekejte</h2>
                        <p>
                            Po několika minutách se objeví následující
                            obrazovka:
                        </p>
                    </v-col>
                </v-row>
                <v-row align="center" justify="end">
                    <v-sheet class="pa-12" color="white">
                        <v-img
                            src="instructions/instructions4.png"
                            max-width="500"
                            class="elevation-4"
                        />
                    </v-sheet>
                </v-row>
            </v-col>
        </v-row>

        <v-row align="center" justify="center">
            <v-col align="center" justify="center">
                <v-row align="center">
                    <v-col>
                        <h1>5.</h1>
                    </v-col>
                    <v-col>
                        <p>
                            Klepněte na <strong>Stáhnout</strong> a pro ověření
                            se znovu přihlaste svým Google účtem. Následně se
                            Vám na disk stáhne soubor s názvem (například)
                            <code>takeout-20200314T194428Z-001.zip</code>
                        </p>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
        <v-row align="center" justify="center">
            <v-col align="center" justify="space-around">
                <v-row align="center">
                    <v-col>
                        <h1>6.</h1>
                    </v-col>
                    <v-col>
                        <h2>Nahrajte soubor</h2>
                        <p>
                            Tento soubor nahrajte na
                            <router-link :to="{ name: 'Home' }"
                                >úvodní stránce</router-link
                            >
                            Děkujeme!
                        </p>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
    </v-col>
</template>

<style scoped></style>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";

@Component({})
export default class Instructions extends Vue {}
</script>
