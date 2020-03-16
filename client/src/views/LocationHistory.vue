<template>
    <v-container class="pa-0">
        <v-row no-gutters class="fill-height-hack">
            <v-col cols="9" class="fill-height">
                <LocationHistoryMap :locations="locations" />
            </v-col>
            <v-col cols="3" class="fill-height">
                <LocationHistorySidePanel :locations="locations" />
            </v-col>
        </v-row>
    </v-container>
</template>

<style scoped>
.fill-height-hack {
    height: calc(100vh - 80px);
}
</style>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import axios from "axios";
import LocationHistoryMap from "@/components/LocationHistoryMap.vue";
import LocationHistorySidePanel from "@/components/LocationHistorySidePanel.vue";
import {Location} from "@/types/Location";

@Component({
    components: {LocationHistorySidePanel, LocationHistoryMap }
})
export default class LocationHistory extends Vue {
    locations: Location[] = [];

    mounted() {
        this.loadLocations();
    }

    async loadLocations() {
        // this.locations = [
        //     {
        //         "dateTime": "1517645260330",
        //         "latitude": 500437725,
        //         "longitude": 144549068,
        //         "accuracy": 96,
        //     },
        //     {
        //         "dateTime": "1517649982844",
        //         "latitude": 500437275,
        //         "longitude": 144545330,
        //         "accuracy": 33,
        //     },
        // ];
        const response = await axios.get(
            `${process.env.VUE_APP_API_URL}/users/${this.$route.params.id}/locations`
        );
        this.locations = response.data;
    }
}
</script>
