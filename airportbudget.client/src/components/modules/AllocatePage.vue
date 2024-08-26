<template>
    <v-container>
        <v-form ref="AllocateFormRef" @submit.prevent="handleSubmit">
            <v-row>
                <v-col cols="12">
                    <v-card outlined>
                        <v-card-title>經費管理</v-card-title>
                        <v-card-subtitle>從哪裡勻出</v-card-subtitle>
                        <v-card-text>
                            <v-row>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.GroupId"
                                              :items="groups" item-title="text" item-value="value"
                                              label="組室別"
                                              :readonly="true"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.Subject6"
                                              :items="subjects6"
                                              item-title="text"
                                              item-value="value"
                                              label="六級(科目)"
                                              :readonly="true"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.Subject7"
                                              :items="subjects7"
                                              item-title="text"
                                              item-value="value"
                                              label="七級(子目)"
                                              :readonly="true"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.Subject8"
                                              :items="subjects8"
                                              item-title="text"
                                              item-value="value"
                                              label="八級(細目)"
                                              :readonly="true"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.RequestPerson"
                                              :items="people"
                                              label="請購人"
                                              :rules="[rules.requestPersonRequired]"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.AmountYear"
                                              :items="years"
                                              label="年度"
                                              :rules="[rules.required]"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-text-field v-model="AllocateForm.RequestDate"
                                                  label="請購日期"
                                                  type="date"
                                                  :rules="[rules.required]"></v-text-field>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-text-field v-model="AllocateForm.RequestAmount"
                                                  label="金額"
                                                  type="number"
                                                  :rules="[rules.required, rules.lessThanOrEqualToBudget]"></v-text-field>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.PaymentPerson"
                                              :items="people"
                                              label="支付人"
                                              :rules="[rules.required]"></v-select>
                                </v-col>
                            </v-row>
                        </v-card-text>
                    </v-card>
                </v-col>

                <v-col cols="12">
                    <v-card outlined>
                        <v-card-subtitle>勻入至哪裡</v-card-subtitle>
                        <v-card-text>
                            <v-row>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.InGroupId"
                                              :items="groups" item-title="text" item-value="value"
                                              label="組室別"
                                              @update:modelValue="fetchSubjects6_in"
                                              :rules="[rules.required]"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.Subject6_in"
                                              :items="Subjects6_in"
                                              label="六級(科目)"
                                              @update:modelValue="fetchSubjects7_in"
                                              :rules="[rules.required, rules.subject6_inRequired]"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.Subject7_in"
                                              :items="Subjects7_in"
                                              label="七級(子目)"
                                              @update:modelValue="fetchSubjects8_in"
                                              :rules="[rules.required, rules.subject7_inRequired]"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.Subject8_in"
                                              :items="Subjects8_in"
                                              label="八級(細目)"
                                              :rules="[rules.subject8_inCheck]"></v-select>
                                </v-col>
                            </v-row>
                            <v-row>

                                <v-col cols="12" md="3">
                                    <v-text-field v-model="AllocateForm.Description"
                                                  label="摘要"
                                                  :rules="[rules.required]"></v-text-field>
                                </v-col>
                            </v-row>

                        </v-card-text>
                        <v-card-actions>
                            <v-spacer></v-spacer>
                            <v-btn text="取消"
                                   color="primary"
                                   variant="outlined"
                                   @click="cancelData"
                                   size="large"
                                   :loading="btnLoading">
                                <template v-slot:loader>
                                    <v-progress-circular indeterminate></v-progress-circular>
                                </template>
                            </v-btn>
                            <v-btn text="確認"
                                   type="submit"
                                   variant="elevated"
                                   color="primary"
                                   class="mr-3"
                                   size="large"
                                   :loading="btnLoading">
                                <template v-slot:loader>
                                    <v-progress-circular indeterminate></v-progress-circular>
                                </template>
                            </v-btn>
                        </v-card-actions>
                    </v-card>
                </v-col>
            </v-row>
        </v-form>
    </v-container>
</template>


<script setup lang="ts">
import axios from 'axios';
import { ref, computed, onMounted, reactive, watch } from 'vue';
import type { AllocateForm, UserDataModel, AllocateFormViewModel, SelectedBudgetModel, UserViewModel, GetIdDataViewModel } from '@/types/apiInterface';
import type { SelectedOption } from '@/types/vueInterface';
//import { groupMapping } from '@/utils/mappings';
import { get, put, post, type ApiResponse } from '@/services/api';
import type { VDataTable } from 'vuetify/components';
import { RULES } from '@/constants/constants';
type ReadonlyHeaders = VDataTable['$props']['headers'];

    const props = defineProps({
        data: {
            type: Object as () => SelectedBudgetModel,
            require: true
        },
        searchYear: {
            type: Number,
            require: true
        }
    });
    //console.log('props.data:', props.data);
    //console.log('year', props.searchYear);
const sourceData: SelectedBudgetModel = props.data!;
const emit = defineEmits(['cancel']);
const AllocateFormRef = ref<HTMLFormElement | null>(null);
const btnLoading = ref<boolean>(false);
const groups = ref<SelectedOption[]>([
    { "text": "請選擇", "value": 0 },
    { "text": "工務組", "value": 1 },
    { "text": "業務組", "value": 2 },
    { "text": "人事室", "value": 3 },
    { "text": "中控室", "value": 4 },
    { "text": "北竿站", "value": 5 },
    { "text": "企劃組", "value": 6 },
    { "text": "南竿站", "value": 7 },
    { "text": "政風室", "value": 8 },
    { "text": "航務組", "value": 9 },
    { "text": "總務組", "value": 10 },
    { "text": "企劃行政組", "value": 11 },
    { "text": "營運安全組", "value": 12 }
]);
const subjects6 = ref<any[]>([{ text: '無', value: "0" }]);
const subjects7 = ref([{ text: '無', value: "0" }]);
const subjects8 = ref([{ text: '無', value: "0" }]);
const Subjects6_in = ref<string[]>(['無']);
const Subjects7_in = ref<string[]>(["無"]);
//const Subjects7_in = ref([{ Subject7Id: 0, text: '無', value: "" }]);
const Subjects8_in = ref<string[]>(["無"]);
const people = ref<string[]>([]);

const currentYear: number = new Date().getFullYear() - 1911; // 取得當年度的民國年
const years = ref<number[]>(Array.from({ length: currentYear - 111 + 1 }, (_, i) => 111 + i));  // 生成從111到當年度的年份陣列

const toUTC = (date: Date) => {
    return new Date(date.getTime() - date.getTimezoneOffset() * 60000);
};
    const defaultUser: UserViewModel = {
        UserId: 0,
        Name: '',
        Account: '',
        Password: '',
        RolePermissionId: 1,
        GroupId: 1,
        Status: true,
        System: undefined,
        LastPasswordChangeDate: toUTC(new Date()),
        ErrCount: 0,
        ErrDate: toUTC(new Date(1990, 0, 1)),
    };

    const defaultAllocateForm: AllocateForm = {
        BudgetAmountId: 0,
        //AmountSerialNumber: 0,
        Status: "O",
        BudgetId: sourceData.BudgetId,
        BudgetName: sourceData.BudgetName!,
        GroupId: sourceData.GroupId,
        InGroupId: 0,
        GroupName: '',
        Subject6: sourceData.Subject6 ?? '',
        Subject7: sourceData.Subject7 ?? '',
        Subject8: sourceData.Subject8 ?? '',
        RequestAmount: 0,
        PaymentAmount: 0,
        Subject6_in: '無',
        Subject7_in: '無',
        Subject8_in: '無',
        RequestPerson: '',
        CreatedYear: props.searchYear!, 
        AmountYear: props.searchYear!,
        RequestDate: '',
        Description: '',
        PaymentPerson: '',
        Remarks: '',
        Type: 0,
        ExTax: false,
        Reconciled: false,
        IsValid: true,
    };

    const groupMapping: Record<number, string> = {
        1: '工務組',
        2: '業務組',
        3: '人事室',
        4: '中控室',
        5: '北竿站',
        6: '企劃組',
        7: '南竿站',
        8: '政風室',
        9: '航務組',
        10: '總務組',
        11: '企劃行政組',
        12: '營運安全組'
    };

    const user = ref<UserViewModel>(defaultUser); 

    const AllocateForm = ref<AllocateForm>({
        ...defaultAllocateForm,
        //GroupId: sourceData.GroupId,
        //BudgetId: sourceData.BudgetId,
        //BudgetName: sourceData.BudgetName!,
        //Subject6: sourceData.Subject6 ?? '',
        //Subject7: sourceData.Subject7 ?? '',
        //Subject8: sourceData.Subject8 ?? '',
        RequestPerson: user.value.Name,
        PaymentPerson: user.value.Name,
    });

    const rules = {
        ...RULES,
        lessThanOrEqualToBudget: (value: number) => {
            return value <= sourceData.UseBudget! || `請購金額不能大於 ${sourceData.UseBudget}`;
        },
        //lessThanOrEqualToPurchaseMoney: (value: number) => {
        //    return value <= limitPurchaseMoney.value || '實付金額不能大於請購金額';
        //}
        requestPersonRequired: (value: string) => {
            return value !== "無" || "請購人不能為 '無'";
        },
        subject6_inRequired: (value: string) => {
            return value !== "無" || "六級(科目)不能為 '無'";
        },
        subject7_inRequired: (value: string) => {
            return value !== "無" || "七級(子目)不能為 '無'";
        },
        subject8_inCheck: (value: string) => {
            // 當 Subjects8_in 有 "無" 以外的值時，檢查是否為 "無"
            const hasValidOption = Subjects8_in.value.some((subject) => subject !== "無");
            if (hasValidOption && value === "無") {
                return "八級(細目)不能為 '無'";
            }
            return true;
        },

    };
    

    const getCurrentUser = async () => {
        const url = '/api/User/Current';
        try {
            const response: ApiResponse<UserViewModel> = await get<UserViewModel>(url);
            if (response.StatusCode === 200) {
                const data = response.Data!;
                user.value = data ? data : defaultUser;
            }
            else {
                console.error(response.Data ?? response.Message);
            }
        }
        catch (error: any) {
            console.error(error.message);
        }
    };

    const fetchPeople = async () => {
        const url = '/api/User';
        try {
            const response: ApiResponse<UserViewModel[]> = await get<UserViewModel[]>(url);  // 假設有一個 API 提供請購人資料
            //console.log('Data:', response.Data);
            if (response.StatusCode == 200) {
                people.value = ["無"].concat(response.Data?.map((person: UserViewModel) => person.Name ?? '') || []);
            }
        } catch (error) {
            console.error('Failed to fetch people:', error);
        }
    };

    //const fetchSubjects6 = async () => {
    //    //console.log('fetchSubjects6 called');
    //    if (!AllocateForm.value.Group) return;
    //    const url = '/api/Subject6/Subjects6';
    //    const data = { group: AllocateForm.value.Group };
    //    try {
    //        const response: ApiResponse<any> = await get<any>(url, data);
    //        if (response.StatusCode == 200) {
    //            //console.log(response.Data);
    //            // 將資料轉換成 { text: Name, value: ID } 的結構 concat方法可以展開陣列
    //            subjects6.value = [{ text: "無", value: "0" }].concat(
    //                response.Data.map((item: { Name: string; ID: string }) => ({
    //                    text: item.Name,
    //                    value: item.ID,
    //                }))
    //            );
    //        }
    //    } catch (error) {
    //        console.error('Failed to fetch subjects6:', error);
    //    }
    //};

    //const fetchSubjects7 = async () => {
    //    if (!AllocateForm.value.Subject6) return;
    //    const url = '/api/Type2/Subjects7';
    //    const data = {
    //        group: AllocateForm.value.Group,
    //        id: AllocateForm.value.Subject6
    //    };
    //    try {
    //        const response: ApiResponse<any> = await get<any>(url, data);
    //        if (response.StatusCode == 200) {
    //            subjects7.value = [{ text: "無", value: "0" }].concat(
    //                response.Data.map((item: { Name: string; ID: string }) => ({
    //                    text: item.Name,
    //                    value: item.ID,
    //                }))
    //            );
    //        }
    //    } catch (error) {
    //        console.error('Failed to fetch subjects7:', error);
    //    }
    //};

    //const fetchSubjects8 = async () => {
    //    if (!AllocateForm.value.Subject7) return;
    //    const url = '/api/Type3/Subjects8';
    //    const data = {
    //        group: AllocateForm.value.Group,
    //        id: AllocateForm.value.Subject7
    //    };
    //    try {
    //        const response: ApiResponse<any> = await get<any>(url, data);
    //        if (response.StatusCode == 200) {
    //            subjects8.value = [{ text: '無', value: '' }].concat(
    //                response.Data?.map((item: { Name: string, ID: string }) => ({
    //                text: item.Name,
    //                value: item.ID
    //            }))) || [];
    //        }
    //    } catch (error) {
    //        console.error('Failed to fetch subjects8:', error);
    //    }
    //};

    const fetchSubjects6_in = async () => {
        AllocateForm.value.Subject6_in = "無";
        AllocateForm.value.Subject7_in = "無";
        AllocateForm.value.Subject8_in = "無";
        Subjects6_in.value = ['無'];
        Subjects7_in.value = ["無"];
        Subjects8_in.value = ["無"];
        if (!AllocateForm.value.InGroupId) return;
        const url = '/api/Budget/GetSubjects6';
        const data = {
            groupId: AllocateForm.value.InGroupId,
            year: AllocateForm.value.CreatedYear,
            subject6: AllocateForm.value.Subject6,
            //subject6: AllocateForm.value.Subject6.substring(0, 2) // 提取前兩個字元
        };
        try {
            const response: ApiResponse<any> = await get<any>(url, data);
            if (response.Data == '這個組室沒有指定科目!') {
                Subjects6_in.value = ["無"];
                Subjects7_in.value = ["無"];
                Subjects8_in.value = ["無"];
                alert(response.Data);
                //AllocateForm.value = {...defaultAllocateForm };
                return;
            }
            if (response.StatusCode == 200) {
                Subjects6_in.value = ['無'].concat(
                    response.Data || []
                );
                //console.log(response.Data);
            }
        } catch (error) {
            console.error('Failed to fetch Subjects6_in:', error);
        }
    };

    const fetchSubjects7_in = async () => {
        AllocateForm.value.Subject7_in = "無";
        AllocateForm.value.Subject8_in = "無";
        Subjects7_in.value = ["無"];
        Subjects8_in.value = ["無"];
        if (!AllocateForm.value.Subject6_in || AllocateForm.value.Subject6_in == "無") return;
        //const selectedItem = Subjects6_in.value.find(item => item.value === selectedSubject6Name);
        const url = '/api/Budget/GetSubjects7';
        const data = {
            groupId: AllocateForm.value.InGroupId,
            year: AllocateForm.value.CreatedYear,
            subject6: AllocateForm.value.Subject6_in
            //subject6Id: selectedItem?.Subject6Id
        };
        try {
            const response: ApiResponse<any> = await get<any>(url, data);
            if (response.StatusCode == 200) {
                Subjects7_in.value = ["無"].concat(
                    response.Data|| []
                );
            }
        } catch (error) {
            console.error('Failed to fetch Subjects7_in:', error);
        }
    };

    const fetchSubjects8_in = async (selectedSubject7Name: string) => {
        AllocateForm.value.Subject8_in = "無";
        Subjects8_in.value = ["無"];
        if (!AllocateForm.value.Subject7_in || AllocateForm.value.Subject7_in == "無") return;
        //const selectedItem = Subjects7_in.value.find(item => item.value === selectedSubject7Name);
        const url = '/api/Budget/GetSubjects8';
        const data = {
            groupId: AllocateForm.value.InGroupId,
            year: AllocateForm.value.CreatedYear,
            subject7: AllocateForm.value.Subject7_in,
            //subject7Id: selectedItem?.Subject7Id
        };
        try {
            const response: ApiResponse<any> = await get<any>(url, data);
            if (response.StatusCode === 200 && Array.isArray(response.Data) && response.Data.length > 0) {
                Subjects8_in.value = ["無"].concat(response.Data);
            } else if (response.StatusCode === 200 && (!Array.isArray(response.Data) || response.Data.length === 0)) {
                console.log('Received empty data, no assignment made to Subjects8_in.');
            }
        } catch (error) {
            console.error('Failed to fetch Subjects8_in:', error);
        }
    };

    const handleSubmit = async () => {
        btnLoading.value = true;
        const { valid } = await AllocateFormRef.value?.validate();
        if (!valid) { btnLoading.value = false; return; };
        if (AllocateForm.value.PaymentPerson == '無') AllocateForm.value.PaymentPerson = '';
        if (AllocateForm.value.Subject8_in == '無') AllocateForm.value.Subject8_in = ''; // 如果Subject8沒有值,設定為空值
        const dataOut = {
            ...AllocateForm.value,
            Type: 2, // 改成帶參數進去
            //Remarks: groupMapping[AllocateForm.value.InGroupId] + `${AllocateForm.value.Subjects7_in + '00'}`,
            GroupId: AllocateForm.value.GroupId,
        };
        const dataIn = {
            ...dataOut,
            Type: 3,
            //Remarks: groupMapping[AllocateForm.value.GroupId] + `${AllocateForm.value.Subject7.substring(0, 4) + '00'}`,
            GroupId: AllocateForm.value.InGroupId
        };

        const idUrl = '/api/Budget/GetBudgetId';
        const getIdDataViewModel: GetIdDataViewModel = {
            GroupId: dataIn.GroupId!,
            Subject6: dataIn.Subject6_in,
            Subject7: dataIn.Subject7_in,
            Subject8: dataIn.Subject8_in ?? "",
            CreatedYear: dataIn.CreatedYear
        }
        try {
            const response: ApiResponse<any> = await get<any>(idUrl, getIdDataViewModel);
            if (response.StatusCode == 200) {
                dataIn.BudgetId = response.Data.BudgetId;
            }
        } catch (error) {
            console.error(error);
        }

        const url = '/api/BudgetAmount/ByAllocateForm'
        const data: any = [
            dataOut,
            dataIn
        ];

        try {
            const response: ApiResponse<any> = await post<any>(url, data);
            if (response.StatusCode == 200) {
                console.log(response.Data || response.Message);
            }
            else {
                alert('資料新增失敗');
            }
        } catch (error) {
            console.error(error);
        } finally {
            emit('cancel');
            btnLoading.value = false;
        }

    };

    const cancelData = () => {
        emit('cancel');
    }

    // 監聽 user.value 的變化並更新 AllocateForm.People
    watch(user, (newValue) => {
        if (newValue.Name) {
            AllocateForm.value.RequestPerson = newValue.Name;
            AllocateForm.value.PaymentPerson = newValue.Name;
        }
    });

    //watch(() => AllocateForm.value.group, async () => {
    //    await fetchSubjects6();
    //});

    onMounted(async () => {
        await getCurrentUser();
        await fetchPeople();
    });

</script>

<style scoped>
 
</style>