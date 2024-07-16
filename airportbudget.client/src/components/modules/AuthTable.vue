<template>
    <v-data-table style="width:100%"
                  :headers="headers"
                  :items="items"
                  :items-per-page="-1"
                  class="elevation-1"
                  :hide-default-footer="true">
    </v-data-table>
</template>

<script setup lang="ts">
    import { ref } from 'vue';
    import type { VDataTable } from 'vuetify/components';
    type ReadonlyHeaders = VDataTable['$props']['headers'];
//寫死的Table
    const headers: ReadonlyHeaders = [
        { title: '權限', align: 'start', sortable: false, key: '權限' },
        { title: '功能', key: '功能' },
        { title: '備註', key: '備註' },
    ];
    
    const items = ref([
        { 權限: 'A', 功能: '新增、編輯、刪除、檢視、勻出入、復原刪除', 備註: '管理者' },
        { 權限: 'B', 功能: '新增、編輯、刪除、檢視、勻出入', 備註: '工務組承辦' },
        { 權限: 'C', 功能: '檢視', 備註: '各單位窗口' },
        { 權限: 'D', 功能: '', 備註: '帳號不開放' },
    ]);

</script>

<style scoped>
    /* 其他樣式 */
</style>
