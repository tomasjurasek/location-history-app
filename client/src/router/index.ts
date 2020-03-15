import Vue from "vue";
import VueRouter from "vue-router";
import Home from "@/views/Home.vue";
import Map from "@/views/Map.vue";
import About from "@/views/About.vue";
import Instructions from "@/views/Instructions.vue";

Vue.use(VueRouter);

const routes = [
    {
        path: "/",
        name: "Home",
        component: Home
    },
    {
        path: "/map",
        name: "Map",
        component: Map
    },
    {
        path: "/about",
        name: "About",
        component: About
    },
    {
        path: "/instructions",
        name: "Instructions",
        component: Instructions
    }

];

const router = new VueRouter({
    routes
});

export default router;
