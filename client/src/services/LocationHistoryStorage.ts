import {LocationHistory} from "@/types/LocationHistory";

export class LocationHistoryStorage {
    private locationHistory?: LocationHistory;

    load(): LocationHistory | undefined {
        return this.locationHistory;
    }

    save(locationHistory: LocationHistory | undefined) {
        this.locationHistory = locationHistory;
    }
}

export const locationHistoryStorage = new LocationHistoryStorage();
