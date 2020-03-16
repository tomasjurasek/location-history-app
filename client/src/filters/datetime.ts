import { format as formatFn, parseISO } from "date-fns";

export function datetime(value?: string) {
    if (value == undefined) {
        return "";
    }
    return formatFn(parseISO(value), "d.M.yyyy H:mm");
}
