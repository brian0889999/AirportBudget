<template>
    <v-row justify="start">
        <v-col cols="12" sm="8" md="6">
            <v-row>
                <v-col cols="3">
                    <v-text-field label="輸入摘要"
                                  v-model="noteInput"
                                  style="width: 100%;" />
                </v-col>
                <v-col cols="3">
                    <v-text-field label="輸入請購金額"
                                  v-model="purchaseMoneyInput"
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

    const noteInput = ref('');
    const purchaseMoneyInput = ref<number | null>(null);
    const emit = defineEmits(['search']);
const searchByInputs = () => {
    // 查詢輸入欄位邏輯
    //console.log('Note:', noteInput.value, 'PurchaseMoney:', purchaseMoneyInput.value);
    // 可以使用 emit 傳遞輸入值到父組件
    emit('search', { Note: noteInput.value, PurchaseMoney: purchaseMoneyInput.value });
}
</script>
