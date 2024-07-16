import { defineStore } from 'pinia';

interface State {
    drawer: boolean,
    rail: boolean,
}

export const useStore = defineStore('pinia',{
    state: (): State => ({
        drawer: true,
        rail: false,
    }),
    actions: {
        setDrawer(value: boolean) {
            this.drawer = value;
        },
        setRail(value: boolean) {
            this.rail = value;
        },
    },
})