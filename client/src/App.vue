<template>
    <v-app>
        <v-container class="topbar">
            <router-link :to="{ name: 'Home' }" class="logo">
                GLOBAL<br />PROJECT
            </router-link>
            <router-link :to="{ name: 'Home' }" class="site-title">
                Google Location History
            </router-link>
        </v-container>

        <v-content>
            <router-view />
        </v-content>
    </v-app>
</template>

<style scoped>
.topbar {
    display: flex;
    flex-direction: row;
    align-items: center;

    height: 80px;
}

.logo,
.site-title {
    padding: 0 16px;
    color: inherit;
    text-decoration: none;
}

.logo {
    padding: 8px 24px;
    margin-left: -12px;
    line-height: 1.2;

    font-size: 14px;
    font-weight: bold;
    border-right: solid 1px black;
}

.site-title {
    font-size: 14px;
}
</style>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";

@Component({})
export default class App extends Vue {}
</script>
