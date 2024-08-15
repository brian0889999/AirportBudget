<template>
    <v-container>
        <!-- 其他內容 -->
        <!--<v-btn @click="openEditDialog(item)">editItem</v-btn>
        <v-dialog v-model="editDialog" max-width="600px">-->
            <v-card>
                <v-card-title>
                    <h2 class="headline">{{ cardTitle }}</h2>
                </v-card-title>
                <v-card-text>
                    <v-form ref="linkBudgetFormRef">
                        <v-row>
                            <v-col cols="12" sm="6" md="4">
                                <v-text-field v-model="data.Budget!.BudgetName"
                                              label="預算名稱"
                                              :rules="[rules.required]"
                                              readonly></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6" md="4">
                                <v-text-field v-model="data.Budget!.Subject6"
                                              label="科目(6級)"
                                              :rules="[rules.required]"
                                              readonly></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6" md="4">
                                <v-text-field v-model="data.Budget!.Subject7"
                                              label="科目(7級)"
                                              :rules="[rules.required]"
                                              readonly></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6" md="4">
                                <v-text-field v-model="data.Budget!.Subject8"
                                              label="科目(8級)"
                                              readonly></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="8">
                                <v-text-field v-model="data.Description"
                                              label="摘要"
                                              :rules="[rules.required]"
                                              readonly></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6" md="3">
                                <v-text-field v-model="data.Budget!.Group!.GroupName"
                                              label="組室"
                                              :rules="[rules.required]"
                                              readonly></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6" md="3">
                                <v-select v-model="data.Type"
                                          label="類別"
                                          :items="typeValues" item-title="text" item-value="value"
                                          bg-color="grey-lighten-1"
                                          :rules="[rules.required]"
                                          readonly></v-select>
                            </v-col>
                            <v-col cols="12" sm="6" md="3">
                                <v-text-field v-model="formattedRequestDate"
                                              label="請購日期"
                                              type="date"
                                              :rules="[rules.required]"
                                              readonly></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6" md="3">
                                <v-text-field v-model="data.RequestAmount"
                                              label="請購金額"
                                              type="number"
                                              readonly></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6" md="3">
                                <v-text-field v-model="formattedPaymentDate"
                                              label="支付日期"
                                              type="date"
                                              readonly></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6" md="3">
                                <v-text-field v-model="data.PaymentAmount"
                                              label="實付金額"
                                              type="number"
                                              readonly></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6" md="3">
                                <v-select v-model="data.RequestPerson"
                                          label="請購人"
                                          readonly
                                          bg-color="grey-lighten-1"
                                          :rules="[rules.required]"></v-select>
                            </v-col>
                            <v-col cols="12" sm="6" md="3">
                                <v-select v-model="data.PaymentPerson"
                                          label="支付人"
                                          bg-color="grey-lighten-1"
                                          readonly></v-select>
                            </v-col>
                            <v-col cols="12" sm="6" md="3">
                                <v-text-field v-model="data.Remarks"
                                              label="備註"
                                              readonly></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6" md="3">
                                <v-checkbox v-model="data.ExTax"
                                            label="未稅"
                                            disabled
                                            class="myColorClass1"></v-checkbox>
                            </v-col>
                            <v-col cols="12" sm="6" md="3">
                                <v-checkbox v-model="data.Reconciled"
                                            label="已對帳"
                                            disabled
                                            class="myColorClass1"></v-checkbox>
                            </v-col>
                            <v-col cols="12" md="3">
                                <v-select v-model="data.AmountYear"
                                          label="年度"
                                          :rules="[rules.required]"
                                          readonly></v-select>
                            </v-col>
                        </v-row>
                    </v-form>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn text="關閉" color="warning" @click="cancelLinkBudgetForm" variant="elevated" size="large" />
                </v-card-actions>
            </v-card>
        <!--</v-dialog>-->
    </v-container>
</template>

<script setup lang="ts">
    import { defineProps, defineEmits, ref, reactive, watch, type PropType, onMounted, computed } from 'vue';
    import { type ApiResponse, get, put, post } from '@/services/api';
    import type { UserDataModel, BudgetAmountViewModel, SelectedDetail, SoftDeleteViewModel, MoneyRawData, UserViewModel } from '@/types/apiInterface';
    import type { SelectedOption } from '@/types/vueInterface';
    import { TypeMapping } from '@/utils/mappings';
    import { RULES } from '@/constants/constants';
    const props = defineProps({
     data:{ 
         type: Object as () => BudgetAmountViewModel,
         required: true
         },
    });
    //const props = defineProps<{
    //    item: SoftDeleteViewModel;
    //    isEdit: boolean;
    //    searchGroup: string;
    //}>();
    const rules = RULES;

    //console.log(props.data);
    const cardTitle = props.data.Type !== 2 ? '勻入資料' : '勻出資料';
    const linkBudgetFormRef = ref<HTMLFormElement | null>(null);
    const typeValues = ref<SelectedOption[]>([
        { text: '一般', value: 1 },
        { text: '勻出', value: 2 },
        { text: '勻入', value: 3 },
    ]);

    const formattedRequestDate = computed<string>({
    get: () => (props.data.RequestDate ? props.data.RequestDate.split('T')[0] : ''),
    set: (value: string) => {
        props.data.RequestDate = value ? value + "T00:00:00" : '';
    }
});

    const formattedPaymentDate = computed<string>({
    get: () => (props.data.PaymentDate ? props.data.PaymentDate.split('T')[0] : ''),
    set: (value: string) => {
        props.data.PaymentDate = value ? value + "T00:00:00" : '';
    }
});
    const emit = defineEmits(['cancel']);

    const cancelLinkBudgetForm = () => {
        emit('cancel');
    }


    //onMounted();
</script>

<style scoped>
    /* 其他樣式 */
    .myColorClass1 {
        background-color: #BDBDBD !important;
    }
    .myColorClass2 {
        /*background-color: #dedede !important;*/
    }
</style>
