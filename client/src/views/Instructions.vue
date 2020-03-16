<template>
  <div class="pa-3">
    <h1>Jak na to?</h1>

    <h2>Přihlaste se do Google Takeout</h2>
    <p>
      <a
        href="https://takeout.google.com/u/0/settings/takeout?pli=1"
        target="_blank"
        >https://takeout.google.com/u/0/settings/takeout?pli=1</a
      ><br />
      Klikněte na <strong>Zrušit výběr všech</strong> a vyberte pouze historii
      polohy.
    </p>
    <v-sheet class="pa-12" color="white">
      <v-img
        src="instructions/instructions1.png"
        max-width="500"
        class="elevation-4"
      />
    </v-sheet>
    <p>
      Nechte formát dat JSON a na konci stránky klepněte na
      <strong>pokračovat</strong>.
    </p>
    <v-sheet class="pa-12" color="white">
      <v-img
        src="instructions/instructions2.png"
        max-width="500"
        class="elevation-4"
      />
      <br />
    </v-sheet>
    <p>
      Nechte si odkaz zaslat e-mailem, zvolte exportovat jednou a klikněte na
      <strong>Vytvořit export</strong>. Objeví se následující obrazovka s
      informací, že dochází ke stahování.
    </p>
    <v-sheet class="pa-12" color="white">
      <v-img
        src="instructions/instructions3.png"
        max-width="500"
        class="elevation-4"
      />
    </v-sheet>
    <h2>Čekejte</h2>
    <p>Po několika minutách se objeví následující obrazovka:</p>
    <v-sheet class="pa-12" color="white">
      <v-img
        src="instructions/instructions4.png"
        max-width="500"
        class="elevation-4"
      />
    </v-sheet>
    <p>
      Klepněte na <strong>Stáhnout</strong> a pro ověření se znovu přihlaste
      svým Google účtem. Následně se Vám na disk stáhne soubor s názvem
      (například) <code>takeout-20200314T194428Z-001.zip</code>
    </p>
    <h2>Nahrajte soubor</h2>
    <p>
      Tento soubor nahrajte na
      <v-btn class="ma-2" :to="{ name: 'Home' }">úvodní stránce</v-btn>.
      Děkujeme!
    </p>
  </div>
</template>

<style scoped></style>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";

@Component({})
export default class Instructions extends Vue {}
</script>
