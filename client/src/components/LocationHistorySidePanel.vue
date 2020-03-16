<template>
    <v-row no-gutters class="flex-column fill-height">
        <v-col class="pa-3 pb-0" style="flex: 0">
            <h2>Detaily</h2>
        </v-col>
        <v-col class="pa-3 overflow-y-auto">
            <v-expansion-panels>
                <v-expansion-panel
                    v-for="(location, index) in locations"
                    :key="index"
                >
                    <v-expansion-panel-header>
                        {{ location.dateTimeUtc | datetime }}
                    </v-expansion-panel-header>
                    <v-expansion-panel-content>
                        Souřadnice: {{ location.latitude / 10 ** 7 }},
                        {{ location.longitude / 10 ** 7 }}
                        <br />
                        Přesnost:
                        {{ location.accuracy ? `${location.accuracy} m` : "-" }}
                    </v-expansion-panel-content>
                </v-expansion-panel>
            </v-expansion-panels>
        </v-col>
    </v-row>
</template>

<style scoped></style>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import { Prop } from "vue-property-decorator";
import { Location } from "@/types/Location";

@Component({})
export default class LocationHistorySidePanel extends Vue {
    @Prop({ default: [] }) locations!: Location[];
}
</script>
