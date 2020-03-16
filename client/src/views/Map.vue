<template>
    <div class="pa-3 fill-height">
        <div ref="map" class="fill-height"></div>
    </div>
</template>

<style scoped>
@import '~mapbox-gl/dist/mapbox-gl.css';
</style>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import mapboxgl from 'mapbox-gl';
import axios from 'axios';

interface Location {
    dateTime: string;
    latitude: number;
    longitude: number;
    accuracy: number;
}

@Component({})
export default class Home extends Vue {
    $refs!: {
        map: HTMLDivElement;
    };

    map!: mapboxgl.Map;

    locations: Location[] = [];

    async mounted() {
        await Promise.all([this.loadMap(), this.loadLocations()]);
        this.renderLocations();
    }

    loadMap() {
        return new Promise(resolve => {
            this.map = new mapboxgl.Map({
                container: this.$refs.map,
                style: 'mapbox://styles/mapbox/streets-v11',
                accessToken: 'pk.eyJ1IjoiemFramFuIiwiYSI6ImNrMzdzMmtvMzAwdDYzY25jN3Fjc29nbTgifQ.WgBeg8tancmrSs-ld3h1Jw',
                center: [15.339133, 49.743700],
                zoom: 7,
            });
            this.map.addControl(new mapboxgl.NavigationControl(), 'top-right');
            this.map.addControl(new mapboxgl.ScaleControl(), 'bottom-right');
            this.map.on('load', resolve);
        });
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
        let response = await axios.get(`${process.env.VUE_APP_API_URL}/users/${this.$route.params.id}/locations`);
        this.locations = response.data;
    }

    renderLocations() {
        this.map.addSource('lines', {
            type: 'geojson',
            data: {
                type: 'FeatureCollection',
                features: [{
                    type: 'Feature',
                    geometry: {
                        type: 'LineString',
                        coordinates: this.locations.map(location => {
                            return [location.longitude / 10 ** 7, location.latitude / 10 ** 7];
                        }),
                    },
                    properties: {},
                }],
            },
        });
        this.map.addSource('points', {
            type: 'geojson',
            data: {
                type: 'FeatureCollection',
                features: this.locations.map(location => {
                    return {
                        type: 'Feature',
                        geometry: {
                            type: 'Point',
                            coordinates: [location.longitude / 10 ** 7, location.latitude / 10 ** 7],
                        },
                        properties: {
                            accuracy: location.accuracy,
                            timestamp: location.dateTime,
                        },
                    };
                }),
            },
        });
        this.map.addLayer({
            id: 'lines',
            type: 'line',
            source: 'lines',
            layout: {},
            paint: {
                'line-color': '#ff0000',
            },
        });
        this.map.addLayer({
            id: 'points',
            type: 'circle',
            source: 'points',
            layout: {},
            paint: {
                'circle-color': '#ff0000',
            },
        });
    }
}
</script>
