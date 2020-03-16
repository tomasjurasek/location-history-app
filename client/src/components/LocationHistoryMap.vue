<template>
    <div ref="map" class="fill-height"></div>
</template>

<style scoped>
@import "~mapbox-gl/dist/mapbox-gl.css";
</style>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import mapboxgl from "mapbox-gl";
import { Prop, Watch } from "vue-property-decorator";
import { Location } from "@/types/Location";

@Component({})
export default class LocationHistoryMap extends Vue {
    $refs!: {
        map: HTMLDivElement;
    };

    map!: mapboxgl.Map;
    mapLoaded = false;

    @Prop({ default: [] }) locations!: Location[];

    async mounted() {
        this.loadMap();
    }

    loadMap() {
        this.map = new mapboxgl.Map({
            container: this.$refs.map,
            style:
                "https://api.maptiler.com/maps/streets/style.json?key=R1lSouzUdcrAwXeY6zJy",
            center: [15.339133, 49.7437],
            zoom: 6
        });
        this.map.addControl(new mapboxgl.NavigationControl(), "top-right");
        this.map.addControl(new mapboxgl.ScaleControl(), "bottom-right");
        this.map.on("load", () => (this.mapLoaded = true));
    }

    @Watch("locations")
    renderLocations() {
        if (!this.map || !this.mapLoaded) {
            setTimeout(() => this.renderLocations(), 200);
            return;
        }

        this.map.addSource("lines", {
            type: "geojson",
            data: {
                type: "FeatureCollection",
                features: [
                    {
                        type: "Feature",
                        geometry: {
                            type: "LineString",
                            coordinates: this.locations.map(location => {
                                return [
                                    location.longitude / 10 ** 7,
                                    location.latitude / 10 ** 7
                                ];
                            })
                        },
                        properties: {}
                    }
                ]
            }
        });
        this.map.addSource("points", {
            type: "geojson",
            data: {
                type: "FeatureCollection",
                features: this.locations.map(location => {
                    return {
                        type: "Feature",
                        geometry: {
                            type: "Point",
                            coordinates: [
                                location.longitude / 10 ** 7,
                                location.latitude / 10 ** 7
                            ]
                        },
                        properties: {
                            dateTimeUtc: location.dateTimeUtc,
                            accuracy: location.accuracy
                        }
                    };
                })
            }
        });
        this.map.addLayer({
            id: "lines",
            type: "line",
            source: "lines",
            layout: {},
            paint: {
                "line-color": "#ff0000"
            }
        });
        this.map.addLayer({
            id: "points",
            type: "circle",
            source: "points",
            layout: {},
            paint: {
                "circle-color": "#ff0000"
            }
        });
    }
}
</script>
