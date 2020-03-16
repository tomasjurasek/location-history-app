import { format as formatFn, parseISO } from "date-fns";

export function date(value?: string) {
    if (value == undefined) {
        return "";
    }
    return formatFn(parseISO(value), "d.M.yyyy");
}
