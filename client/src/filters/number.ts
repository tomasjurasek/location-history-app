export function number(value?: number) {
    if (value == undefined) {
        return "";
    }
    return value.toLocaleString();
}
