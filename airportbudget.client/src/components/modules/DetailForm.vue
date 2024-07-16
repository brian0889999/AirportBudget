<template>
    <v-container>
        <!-- 其他內容 -->
        <!--<v-btn @click="openEditDialog(item)">editItem</v-btn>
        <v-dialog v-model="editDialog" max-width="600px">-->
            <v-card>
                <v-card-title>
                    <span class="headline">編輯項目</span>
                </v-card-title>
                <v-card-text>
                    <v-form ref="editFormRef" v-model="isValid">
                        <v-row>
                            <v-col cols="12" sm="6">
                                <v-text-field v-model="formattedPurchasedate"
                                              label="請購日期"
                                              type="date"
                                              :rules="[rules.required]"></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-select v-model="editedItem.Text"
                                          label="類別"
                                          :items="textValues"
                                          :bg-color="textColor"
                                          :rules="[rules.required]"
                                          :readonly="text"></v-select>
                            </v-col>
                            <v-col cols="12">
                                <v-text-field v-model="editedItem.Note"
                                              label="摘要"
                                              :rules="[rules.required]"></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-text-field v-model="editedItem.PurchaseMoney"
                                              label="請購金額"
                                              type="number"
                                              :rules="[rules.lessThanOrEqualToBudget]"></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-text-field v-model="formattedPayDate"
                                              label="支付日期"
                                              type="date"
                                              ></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-text-field v-model="editedItem.PayMoney"
                                              label="實付金額"
                                              type="number"
                                              :rules="[rules.lessThanOrEqualToPurchaseMoney]"></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-select v-model="editedItem.People"
                                          :items="userNames"
                                          label="請購人"
                                          :readonly="isStatusC"
                                          :bg-color="getTextColor(isStatusC)"
                                          :rules="[rules.required]"></v-select>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-select v-model="editedItem.People1"
                                          :items="userNames"
                                          label="支付人"
                                          ></v-select>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-text-field v-model="editedItem.Remarks"
                                              label="備註"></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-checkbox v-model="editedItem.All"
                                            label="未稅"
                                            :disabled="isStatusC"
                                            :class="getColourDisabled(isStatusC)"
                                            true-value="V"
                                            false-value=""></v-checkbox>
                            </v-col>
                            <v-col cols="12" sm="6">
                                <v-checkbox v-model="editedItem.True"
                                            label="已對帳"
                                            :disabled="isStatusC"
                                            :class="getColourDisabled(isStatusC)"
                                            true-value="V"
                                            false-value=""></v-checkbox>
                            </v-col>
                            <v-col cols="12">
                                <v-select v-model="editedItem.Year1"
                                          :items="year1"
                                          label="年度"
                                          :rules="[rules.required]"></v-select>
                            </v-col>
                        </v-row>
                    </v-form>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" text="true" @click="canceledit">取消</v-btn>
                    <v-btn color="blue darken-1" text="true" @click="submitform">{{ saveBtn }}</v-btn>
                </v-card-actions>
            </v-card>
        <!--</v-dialog>-->
    </v-container>
</template>

<script setup lang="ts">
    import { defineProps, defineEmits, ref, reactive, watch, type PropType, onMounted, computed } from 'vue';
    import { type ApiResponse, get, put, post } from '@/services/api';
    import type { UserDataModel, SoftDeleteViewModel, MoneyRawData, UserViewModel } from '@/types/apiInterface';
    import { RULES } from '@/constants/constants';
    const props = defineProps({
        item: {
            type: Object as PropType<MoneyRawData>,
            required: true
        },
        isEdit: {
            type: Boolean,
            required: true
        },
        searchGroup: {
            type: String,
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
    const editFormRef = ref<HTMLFormElement | null>(null);
    const saveBtn = props.isEdit ? '儲存' : '新增';
    const text = props.isEdit ? true : false;
    const textColor = props.isEdit ? 'grey-lighten-1' : '';
    const textValues = ref<string[]>(['一般']);
    const emit = defineEmits(['update', 'cancel', 'create']);

    const isStatusC = computed(() => props.user.Status1 === 'C'); // 使用者權限是C,return true
    const limitBudget = computed(() => props.limitBudget ?? 0);
    const limitPurchaseMoney = computed(() => {
        const value = editedItem.value.PurchaseMoney;
        return typeof value === 'string' ? parseFloat(value) : value ?? 0; // 轉成數字
    });

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
            return value <= limitBudget.value || `請購金額不能大於 ${limitBudget.value}`;
        },
        lessThanOrEqualToPurchaseMoney: (value: number) => {
            return value <= limitPurchaseMoney.value || '實付金額不能大於請購金額';
        }
    };


    const editedItem = ref<MoneyRawData>({ ...props.item });
    watch(() => props.item, (newValue) => {
        editedItem.value = { ...newValue };
    });
    //console.log('props.item', props.item);
    //console.log('editedItem', editedItem.value);
    //console.log('props.searchGroup:', props.searchGroup);

    const users = ref<UserDataModel[]>([]);
    const userNames = ref<string[]>([]);
    const year1 = ref<string[]>(['111', '112', '113']);
    const fetchUsers = async () => {
        try {
            const url = 'api/User';
            const response: ApiResponse<UserDataModel[]> = await get<UserDataModel[]>(url);
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
  const formattedPurchasedate = computed<string>({
    get: () => (editedItem.value.Purchasedate ? editedItem.value.Purchasedate.split('T')[0] : ''),
    set: (value: string) => {
        editedItem.value.Purchasedate = value ? value + "T00:00:00" : '';
    }
});

const formattedPayDate = computed<string>({
    get: () => (editedItem.value.PayDate ? editedItem.value.PayDate.split('T')[0] : ''),
    set: (value: string) => {
        editedItem.value.PayDate = value ? value + "T00:00:00" : '';
    }
});

    const submitform = async () => {
        const { valid } = await editFormRef.value?.validate();
        if (!valid) return;

        // DB的Year1欄位存字串
        const data: SoftDeleteViewModel = {
            ...editedItem.value,
            Year1: editedItem.value.Year1 ? editedItem.value.Year1.toString() : "",
            PayMoney: editedItem.value.PayMoney ? Number(editedItem.value.PayMoney) : 0,
            PurchaseMoney: editedItem.value.PurchaseMoney ? Number(editedItem.value.PurchaseMoney) : 0
        };

        const url = '/api/Money3';
        try {
            //console.log('123', data);
            let response: ApiResponse<any>;
            if (data.ID1) {
                //console.log('345', data);
                response = await put<any>(url, data);
                //console.log(response?.Data || response?.Message);
                // 更新成功後的處理
                emit('update', editedItem.value);
            } else {
                // 在這裡將Year欄位賦值為Year1的值
                data.Year = editedItem.value.Year1 ? parseInt(editedItem.value.Year1, 10) : 0;
                if (!data.PayDate) data.PayDate = null; // 如果PayDate沒有值,傳空值
                console.log('adding data:',data);
                response = await post<any>(url, data);
                //console.log('response.Data:', response?.Data);
                //console.log(response?.Data || response?.Message);
                //console.log('data:', data);
                //console.log(editedItem.value);
                emit('create', editedItem.value);
            }
        } catch (error: any) {
            console.error(error);
        } finally {
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
