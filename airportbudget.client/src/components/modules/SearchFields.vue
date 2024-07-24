<template>
    <v-row justify="start">
        <v-col cols="12" sm="8" md="6">
            <v-row>
                <v-col cols="3">
                    <v-text-field label="輸入摘要"
                                  v-model="descriptionInput"
                                  style="width: 100%;" />
                </v-col>
                <v-col cols="3">
                    <v-text-field label="請購金額起始"
                                  v-model="requestAmountStartInput"
                                  type="number"
                                  style="width: 100%;" />
                </v-col>
                <v-col cols="3">
                    <v-text-field label="請購金額結束"
                                  v-model="requestAmountEndInput"
                                  type="number"
                                  style="width: 100%;" />
                </v-col>
                <v-col cols="3">
                    <v-btn @click="searchByInputs"
                           color="primary"
                           class="mt-2"
                           size="large">
                        查詢
                    </v-btn>
                </v-col>
            </v-row>
        </v-col>
    </v-row>
</template>

<script setup lang="ts">
    import { defineProps, defineEmits, ref } from 'vue';

    const descriptionInput = ref('');
    const requestAmountStartInput = ref<number | null>(null);
    const requestAmountEndInput = ref<number | null>(null);
    const emit = defineEmits(['search']);
const searchByInputs = () => {
    // 查詢輸入欄位邏輯
    //console.log('Description:', descriptionInput.value, 'RequestAmountStart:', requestAmountStartInput.value, 'RequestAmountEnd:', requestAmountEndInput.value);
    // 可以使用 emit 傳遞輸入值到父組件
    emit('search', {
        Description: descriptionInput.value,
        RequestAmountStart: requestAmountStartInput.value,
        RequestAmountEnd: requestAmountEndInput.value
    });
}
</script>
