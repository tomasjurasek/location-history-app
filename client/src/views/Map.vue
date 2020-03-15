<template>
    <div class="pa-3 fill-height">
        <MglMap
            ref="mglMap"
            :center="[15.339133, 49.743700]"
            :zoom="7"
            :access-token="accessToken"
            :map-style="mapStyle"
            :mapbox-gl="mapbox"
            @load="onLoad"
        >
            <MglNavigationControl position="top-right" />
            <MglScaleControl position="bottom-right" />

            <MglGeojsonLayer
                source-id="linesSource"
                :source="linesSource"
                :layer-id="linesLayer.id"
                :layer="linesLayer"
            />
            <MglGeojsonLayer
                source-id="pointsSource"
                :source="pointsSource"
                :layer-id="pointsLayer.id"
                :layer="pointsLayer"
            />
        </MglMap>
    </div>
</template>

<style scoped>
@import '~mapbox-gl/dist/mapbox-gl.css';
</style>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import Mapbox from 'mapbox-gl';
// @ts-ignore
import { MglMap, MglNavigationControl, MglScaleControl, MglGeojsonLayer } from 'vue-mapbox';

@Component({
    components: { MglMap, MglNavigationControl, MglScaleControl, MglGeojsonLayer },
})
export default class Home extends Vue {
    $refs!: {
        mglMap: MglMap;
    };

    accessToken = 'pk.eyJ1IjoiemFramFuIiwiYSI6ImNrMzdzMmtvMzAwdDYzY25jN3Fjc29nbTgifQ.WgBeg8tancmrSs-ld3h1Jw';
    mapStyle = 'mapbox://styles/mapbox/streets-v11';

    mapbox = Mapbox;

    locations = [
        {
            "timestampMs": "1517645260330",
            "latitudeE7": 500437725,
            "longitudeE7": 144549068,
            "accuracy": 96,
        },
        {
            "timestampMs": "1517649982844",
            "latitudeE7": 500437275,
            "longitudeE7": 144545330,
            "accuracy": 33,
        },
    ];
    linesSource: Mapbox.GeoJSONSourceRaw = {
        type: 'geojson',
        data: {
            type: 'FeatureCollection',
            features: [{
                type: 'Feature',
                geometry: {
                    type: 'LineString',
                    coordinates: this.locations.map(location => [location.longitudeE7 / 10**7, location.latitudeE7 / 10**7]),
                },
                properties: {},
            }],
        },
    };
    pointsSource: Mapbox.GeoJSONSourceRaw = {
        type: 'geojson',
        data: {
            type: 'FeatureCollection',
            features: this.locations.map(location => {
                return {
                    type: 'Feature',
                    geometry: {
                        type: 'Point',
                        coordinates: [location.longitudeE7 / 10**7, location.latitudeE7 / 10**7],
                    },
                    properties: {
                        accuracy: location.accuracy,
                        timestamp: location.timestampMs,
                    },
                };
            }),
        },
    };
    linesLayer: Mapbox.Layer = {
        id: 'linesLayer',
        type: 'line',
        layout: {},
        paint: {
            'line-color': '#ff0000',
        },
    };
    pointsLayer: Mapbox.Layer = {
        id: 'pointsLayer',
        type: 'circle',
        layout: {},
        paint: {
            'circle-color': '#ff0000',
        },
    };

    onLoad() {
        console.log(this.linesSource, this.pointsSource);
    }
}
</script>
