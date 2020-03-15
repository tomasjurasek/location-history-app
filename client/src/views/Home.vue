<template>
    <div class="pa-3">
        <p>Historii polohy stáhněte z Google podle <a href="/navod">návodu</a>, výsledný soubor je nazvaný např. <em>takeout-20200315T062605Z-001.zip</em>, ten nahrajte zde.</p>
        <v-form>
            <v-file-input accept=".zip" label="Soubor"></v-file-input>
            <v-btn>Nahrát</v-btn>
        </v-form>
    </div>
</template>

<style scoped></style>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';

@Component({})
export default class Home extends Vue {
}
</script>
