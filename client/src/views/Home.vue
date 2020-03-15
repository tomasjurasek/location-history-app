<template>
    <div class="pa-3">
        <p>Historii polohy stáhněte z Google podle <v-btn text :to="{ name: 'Instructions' }" color="primary">návodu</v-btn>, výsledný soubor je nazvaný např. <em>takeout-20200315T062605Z-001.zip</em>, ten nahrajte zde.</p>
        <v-form>
            <v-file-input accept=".zip" label="Soubor"></v-file-input>
            <v-btn class="ma-2">Nahrát</v-btn>
            <v-btn class="ma-2" :to="{ name: 'Instructions' }">Jak připravit data?</v-btn>
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
