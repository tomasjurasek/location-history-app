import Vue from "vue";
import { date } from "./date";
import { datetime } from "./datetime";
import { number } from "./number";

Vue.filter("date", date);
Vue.filter("datetime", datetime);
Vue.filter("number", number);
