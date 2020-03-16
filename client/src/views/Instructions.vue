<template>
    <v-row align="center" justify="center" no-gutters>
        <v-col md="7">
            <h1>Návod jak stáhnout data z Google</h1>

            <section class="step">
                <header class="step__header">
                    <div class="step__number">1.</div>
                    <div>
                        <h2 class="step__title strong">
                            Přihlaste se do
                            <a
                                href="https://takeout.google.com/u/0/settings/takeout?pli=1"
                                target="_blank"
                                >Google Takeout</a
                            >
                        </h2>
                        <p class="step__description">
                            Klikněte na <strong>Zrušit výběr všech</strong> a
                            vyberte pouze historii polohy.
                        </p>
                    </div>
                </header>
                <v-sheet class="step__image" color="white">
                    <v-img
                        src="instructions/instructions1.png"
                        max-width="500"
                        class="elevation-4"
                    />
                </v-sheet>
            </section>

            <section class="step">
                <header class="step__header">
                    <div class="step__number">2.</div>
                    <div>
                        <h2 class="step__title strong">
                            Nechte formát dat JSON
                        </h2>
                        <p class="step__description">
                            a na konci stránky klepněte na
                            <strong>pokračovat</strong>.
                        </p>
                    </div>
                </header>
            </section>

            <section class="step">
                <header class="step__header">
                    <div class="step__number">3.</div>
                    <div>
                        <h2 class="step__title">
                            Nechte si odkaz zaslat e-mailem, zvolte exportovat
                            jednou a klikněte na
                            <strong>Vytvořit export</strong>.
                        </h2>
                        <p class="step__description">
                            Objeví se následující obrazovka s informací, že
                            dochází ke stahování.
                        </p>
                    </div>
                </header>
                <v-sheet class="step__image" color="white">
                    <v-img
                        src="instructions/instructions3.png"
                        max-width="500"
                        class="elevation-4"
                    />
                </v-sheet>
            </section>

            <section class="step">
                <header class="step__header">
                    <div class="step__number">4.</div>
                    <div>
                        <h2 class="step__title">Čekejte</h2>
                        <p class="step__description">
                            Po několika minutách se objeví následující obrazovka
                        </p>
                    </div>
                </header>
                <v-sheet class="step__image" color="white">
                    <v-img
                        src="instructions/instructions4.png"
                        max-width="500"
                        class="elevation-4"
                    />
                </v-sheet>
            </section>

            <section class="step">
                <header class="step__header">
                    <div class="step__number">5.</div>
                    <div>
                        <h2 class="step__title">
                            Klepněte na <strong>Stáhnout</strong> a pro ověření
                            se znovu přihlaste svým Google účtem.
                        </h2>
                        <p class="step__description">
                            Následně se Vám na disk stáhne soubor s názvem
                            (například)
                            <code>takeout-20200314T194428Z-001.zip</code>
                        </p>
                    </div>
                </header>
                <v-sheet class="step__image" color="white">
                    <v-img
                        src="instructions/instructions5.png"
                        max-width="500"
                        class="elevation-4"
                    />
                </v-sheet>
            </section>
        </v-col>
    </v-row>
</template>

<style scoped>
h1 {
    margin: 32px 0;
    font-size: 32px;
    font-weight: 000;
    text-align: center;
}

.step {
    max-width: 500px;
    margin: 0 auto;
}

.step__header {
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-top: 32px;
    margin-bottom: 16px;
}

.step__number {
    margin-right: 32px;
    font-size: 64px;
    font-weight: 900;
}

.step__title {
    font-size: 14px;
}

.step__title:not(.strong) {
    font-weight: normal;
}

.step__description {
    margin: 0;
    font-size: 14px;
    font-weight: normal;
    opacity: 0.8;
}

.step__image {
    margin-top: 16px;
}
</style>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";

@Component({})
export default class Instructions extends Vue {}
</script>
