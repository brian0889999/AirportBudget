<template>
    <div :class="[level && 'sub-bar-item', rail && 'sub-bar-item-rail']">
        <v-list-item v-if="!item.childs"
                     :prepend-icon="item.icon"
                     :title="item.title"
                     :value="item.href"
                     :to="item.href"
                     active-class="bg-primary text-white"></v-list-item>
        <v-list-group v-else>
            <template v-slot:activator="{ props }">
                <v-list-item v-bind="props"
                             :prepend-icon="item.icon"
                             :title="item.title"></v-list-item>
            </template>
            <AppDrawerItem v-for="child in item.childs"
                           :item="child"
                           :level="level + 1" />
        </v-list-group>
    </div>
</template>


<script setup lang="ts">
    import { ref, computed, defineProps } from 'vue';
    import { useStore } from '@/store/index';
    import type { Crumb } from '@/types/vueInterface';

    import AppDrawerItem from './AppDrawerItem.vue';

    const props = defineProps({
        item: {
            type: Object as () => Crumb,
            required: true,
        },
        level: {
            type: Number,
            default: 0,
        },
    });

    const store = useStore();
    const rail = computed(() => store.rail);
</script>


<style lang="scss" scoped>
    // 覆蓋掉原本的版型
    .v-list-group__items .sub-bar-item .v-list-item {
        padding-inline-start: 8px !important;
    }

    .sub-bar-item {
        padding-left: 24px;
        transition: padding 0.2s;
    }

    .sub-bar-item-rail {
        padding-left: 0px !important;
        transition: padding 0.2s;
    }
</style>
