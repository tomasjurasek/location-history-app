<template>
    <div class="pa-3 fill-height">
        <LocationHistoryMap :locations="locations" />
    </div>
</template>

<style scoped></style>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import axios from "axios";
import LocationHistoryMap from "@/components/LocationHistoryMap.vue";

interface Location {
    dateTime: string;
    latitude: number;
    longitude: number;
    accuracy: number;
}

@Component({
    components: { LocationHistoryMap }
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
