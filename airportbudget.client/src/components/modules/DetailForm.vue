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
                    <v-form ref="editFormRef" v-model="isValid">
                        <v-row>
                            <v-col cols="12" sm="6">
                                <v-text-field v-model="formattedRequestDate"
                                              label="請購日期"
                                              type="date"
                                              :rules="[rules.required]"></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-select v-model="editedItem.Type"
                                          label="類別"
                                          :items="typeValues" item-title="text" item-value="value"
                                          :bg-color="typeColor"
                                          :rules="[rules.required]"
                                          :readonly="type"></v-select>
                            </v-col>
                            <v-col cols="12">
                                <v-text-field v-model="editedItem.Description"
                                              label="摘要"
                                              :rules="[rules.required]"></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-text-field v-model="editedItem.RequestAmount"
                                              label="請購金額"
                                              type="number"
                                              :rules="[rules.lessThanOrEqualToBudget]"></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-text-field v-model="formattedPaymentDate"
                                              label="支付日期"
                                              type="date"
                                              ></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-text-field v-model="editedItem.PaymentAmount"
                                              label="實付金額"
                                              type="number"
                                              :rules="[rules.lessThanOrEqualToRequestAmount]"></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-select v-model="editedItem.RequestPerson"
                                          :items="userNames"
                                          label="請購人"
                                          :readonly="IsPermissionId2"
                                          :bg-color="getTextColor(IsPermissionId2)"
                                          :rules="[rules.required]"></v-select>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-select v-model="editedItem.PaymentPerson"
                                          :items="userNames"
                                          label="支付人"
                                          ></v-select>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-text-field v-model="editedItem.Remarks"
                                              label="備註"></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-checkbox v-model="editedItem.ExTax"
                                            label="未稅"
                                            :disabled="IsPermissionId2"
                                            :class="getColourDisabled(IsPermissionId2)"></v-checkbox>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-checkbox v-model="editedItem.Reconciled"
                                            label="已對帳"
                                            :disabled="IsPermissionId2"
                                            :class="getColourDisabled(IsPermissionId2)"></v-checkbox>
                            </v-col>
                            <v-col cols="12">
                                <v-select v-model="editedItem.AmountYear"
                                          :items="years"
                                          label="年度"
                                          :rules="[rules.required]"></v-select>
                            </v-col>
                        </v-row>
                    </v-form>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <!--<v-btn color="blue darken-1" text="true" @click="canceledit">取消</v-btn>-->
                    <v-btn color="primary" text="取消" :loading="loading" @click="canceledit" variant="outlined" size="large">
                        <template v-slot:loader>
                            <v-progress-circular indeterminate></v-progress-circular>
                        </template>
                    </v-btn>
                    <v-btn color="primary" :text="saveBtn" :loading="loading" @click="submitform" variant="elevated" size="large">
                        <template v-slot:loader>
                            <v-progress-circular indeterminate></v-progress-circular>
                        </template>
                    </v-btn>
                </v-card-actions>
            </v-card>
        <!--</v-dialog>-->
    </v-container>
</template>

<script setup lang="ts">
    import { defineProps, defineEmits, ref, reactive, watch, type PropType, onMounted, computed } from 'vue';
    import { type ApiResponse, get, put, post } from '@/services/api';
    import type { UserDataModel, BudgetAmountViewModel, SelectedDetail, UserViewModel } from '@/types/apiInterface';
    import type { SelectedOption } from '@/types/vueInterface';
    import { TypeMapping } from '@/utils/mappings';
    import { RULES } from '@/constants/constants';
    import { Message } from '@/store/message';
    const props = defineProps({
        item: {
            type: Object as PropType<BudgetAmountViewModel>,
            required: true
        },
        isEdit: {
            type: Boolean,
            required: true
        },
        searchGroup: {
            type: Number,
            //required: true
        },
        limitBudget: {
            type: Number,
        },
        user: {
            type: Object as PropType<UserViewModel>,
            required: true
        },
    });
    //const props = defineProps<{
    //    item: SoftDeleteViewModel;
    //    isEdit: boolean;
    //    searchGroup: string;
    //}>();
    const loading = ref<boolean>(false);
    const editFormRef = ref<HTMLFormElement | null>(null);
    const saveBtn = props.isEdit ? '儲存' : '新增';
    const type = props.isEdit ? true : false;
    const typeColor = props.isEdit ? 'grey-lighten-1' : '';
    const typeValues = ref<SelectedOption[]>([
        { text: '一般', value: 1 },
        //{ text: '勻出', value: 2 },
        //{ text: '勻入', value: 3 },
    ]);
    const emit = defineEmits(['update', 'cancel', 'create']);
    const cardTitle = props.isEdit ? '編輯預算資料' : '新增預算資料';
    const IsPermissionId2 = computed(() => props.user.RolePermissionId === 2); // 使用者權限是2,return true
    const limitBudget = computed(() => props.limitBudget ?? 0);
    const limitRequestAmount = computed(() => {
        const value = editedItem.value.RequestAmount;
        return typeof value === 'string' ? parseFloat(value) : value ?? 0; // 轉成數字
    });

    const initialRequestAmount = ref<number>(0); // 用來儲存初始的請購金額

    // 定義函數
    const getTextColor = (boolean: boolean): string => {
        return boolean ? 'grey-lighten-1' : '';
    };
    const getColourDisabled = (disabledValue: boolean) => {
        if (disabledValue) {
            return "myColorClass1"
        } else {
            return "myColorClass2"
        }
    };
    const rules = {
        ...RULES,
        lessThanOrEqualToBudget: (value: number) => {
            const availableBudget = limitBudget.value + initialRequestAmount.value; // 考慮已存在的金額
            return value <= availableBudget || `請購金額不能大於 ${availableBudget}`;
        },
        lessThanOrEqualToRequestAmount: (value: number) => {
            return value <= limitRequestAmount.value || '實付金額不能大於請購金額';
        }
    };


    const editedItem = ref<BudgetAmountViewModel>({ ...props.item });
    watch(() => props.item, (newValue) => {
        editedItem.value = { ...newValue };
    });
    //console.log('props.item', props.item);
    //console.log('editedItem', editedItem.value);
    //console.log('props.searchGroup:', props.searchGroup);

    const users = ref<UserViewModel[]>([]);
    const userNames = ref<string[]>([]);
    // 取得當年度的民國年
    const currentYear: number = new Date().getFullYear() - 1911;
    // 生成從111到當年度的年份陣列
    const years = ref<number[]>(Array.from({ length: currentYear - 111 + 1 }, (_, i) => 111 + i));
    const fetchUsers = async () => {
        try {
            const url = 'api/User';
            const response: ApiResponse<UserViewModel[]> = await get<UserViewModel[]>(url);
                if(response.StatusCode == 200) {
                    users.value = response.Data!;
                    /*console.log('users:', users);*/
                     // 提取 Name 欄位，並在最前面加入 '無' 選項
                    userNames.value = ['無', ...users.value.map(user => user.Name || '')];
                    //console.log('userNames:', userNames);
            }
        }
        catch (error) {
            console.error(error);
        }
    };
    //console.log('limitBudget', props.limitBudget);
    const formattedRequestDate = computed<string>({
    get: () => (editedItem.value.RequestDate ? editedItem.value.RequestDate.split('T')[0] : ''),
    set: (value: string) => {
        editedItem.value.RequestDate = value ? value + "T00:00:00" : '';
    }
});

    const formattedPaymentDate = computed<string>({
    get: () => (editedItem.value.PaymentDate ? editedItem.value.PaymentDate.split('T')[0] : ''),
    set: (value: string) => {
        editedItem.value.PaymentDate = value ? value + "T00:00:00" : '';
    }
});

    const submitform = async () => {
        const { valid } = await editFormRef.value?.validate();
        if (!valid) return;

        const data: SelectedDetail = {
            ...editedItem.value,
            //PaymentAmount: editedItem.value.PaymentAmount ? Number(editedItem.value.PaymentAmount) : 0,
            //RequestAmount: editedItem.value.RequestAmount ? Number(editedItem.value.RequestAmount) : 0
        };

        let url = '/api/BudgetAmount';
        try {
            loading.value = true;
            let response: ApiResponse<any>;
            if (data.BudgetAmountId) {
                if (!data.PaymentDate) data.PaymentDate = undefined;
                //if (data.Type !== 1) url = '/api/BudgetAmount/ByUpdateAllocate';
                response = await put<any>(url, data);
                //console.log(response?.Data || response?.Message);
                // 更新成功後的處理
                emit('update', editedItem.value);
                if (response.StatusCode == 200 || response.StatusCode == 201) {
                    Message.success('資料更新成功');
                } else {
                    Message.error(response.Data || response.Message);
                }
            } else {
                // 在這裡將CreatedYear欄位賦值為AmountYear的值
                //data.CreatedYear = editedItem.value.AmountYear ? parseInt(editedItem.value.AmountYear, 10) : 0;
                //data.CreatedYear = editedItem.value.AmountYear ? editedItem.value.AmountYear : 0;
                //console.log('adding data:',data);
                response = await post<any>(url, data);
                //console.log('response.Data:', response?.Data);
                //console.log(response?.Data || response?.Message);
                //console.log('data:', data);
                //console.log(editedItem.value);
                emit('create', editedItem.value);
                if (response.StatusCode == 200 || response.StatusCode == 201) {
                    Message.success('資料新增成功');
                } else {
                    Message.error(response.Data || response.Message);
                }
            }
        } catch (error: any) {
            Message.error(error.message);
            console.error(error);
        } finally {
            loading.value = false;
            canceledit();
        }
    };

    const canceledit = () => {
        emit('cancel');
    };

    //const editDialog = ref(false);
    const isValid = ref(false);
    //const editedItem = reactive({
    //    purchaseDate: '',
    //    category: '',
    //    summary: '',
    //    purchaseAmount: 0,
    //    paymentDate: '',
    //    actualPaymentAmount: 0,
    //    requestor: '',
    //    payer: '',
    //    remarks: '',
    //    excludingTax: false,
    //    reconciled: false,
    //    year: 111
    //});


    //const openEditDialog = (item: any) => {
    //    Object.assign(editedItem, item);
    //    editDialog.value = true;
    //};

    //const closeEditDialog = () => {
    //    editDialog.value = false;
    //};

    //const saveEdit = () => {
    //    if ((refs.editForm as any).validate()) {
    //        // 進行儲存操作
    //        closeEditDialog();
    //    }
    //};
    onMounted(fetchUsers);
    onMounted(() => {
        initialRequestAmount.value = editedItem.value.RequestAmount ?? 0;
    });
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
