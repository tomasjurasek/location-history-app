<template>
    <v-app>
        <error-dialog v-model="error" />

        <v-app-bar app flat height="80">
            <v-toolbar-title>
                <router-link :to="{ name: 'Home' }">
                    <v-icon class="mr-3">mdi-medical-bag</v-icon>
                    <span class="secondary--text">Analýza historie polohy</span>
                </router-link>
            </v-toolbar-title>
            <v-spacer></v-spacer>
            <v-toolbar-items>
                <v-btn text icon :to="{ name: 'About' }">
                    <v-icon>mdi-information</v-icon>
                </v-btn>
            </v-toolbar-items>
        </v-app-bar>

        <v-content>
            <router-view />
        </v-content>
    </v-app>
</template>

<style scoped>
.v-app-bar a {
    text-decoration: none;
}

.v-app-bar img {
    vertical-align: middle;
    margin-right: 8px;
}
</style>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';

@Component({})
export default class App extends Vue {
}
</script>
