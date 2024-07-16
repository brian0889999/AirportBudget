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
                                    <v-select v-model="AllocateForm.Group"
                                              :items="groups"
                                              label="組室別"
                                              :readonly="true"
                                              @update:modelValue="fetchSubjects6"
                                              :rules="[rules.required]"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.Subject6"
                                              :items="subjects6"
                                              item-title="text"
                                              item-value="value"
                                              label="六級(科目)"
                                              :readonly="true"
                                              @update:modelValue="fetchSubjects7"
                                              :rules="[rules.required]"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.Subject7"
                                              :items="subjects7"
                                              item-title="text"
                                              item-value="value"
                                              label="七級(子目)"
                                              :readonly="true"
                                              @update:modelValue="fetchSubjects8"></v-select>
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
                                    <v-select v-model="AllocateForm.People"
                                              :items="people"
                                              label="請購人"
                                              :rules="[rules.required]"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.Year"
                                              :items="years"
                                              label="年度"
                                              :rules="[rules.required]"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-text-field v-model="AllocateForm.Purchasedate"
                                                  label="請購日期"
                                                  type="date"
                                                  :rules="[rules.required]"></v-text-field>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-text-field v-model="AllocateForm.PurchaseMoney"
                                                  label="金額"
                                                  type="number"
                                                  :rules="[rules.required, rules.lessThanOrEqualToBudget]"></v-text-field>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.People1"
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
                                    <v-select v-model="AllocateForm.Group1"
                                              :items="groups"
                                              label="組室別"
                                              @update:modelValue="fetchSubjects6_1"
                                              :rules="[rules.required]"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.Subject6_1"
                                              :items="subjects6_1"
                                              item-title="text"
                                              item-value="value"
                                              label="六級(科目)"
                                              @update:modelValue="fetchSubjects7_1"
                                              :rules="[rules.required]"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.Subject7_1"
                                              :items="subjects7_1"
                                               item-title="text"
                                               item-value="value"
                                              label="七級(子目)"
                                              @update:modelValue="fetchSubjects8_1"
                                              :rules="[rules.required]"></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="AllocateForm.Subject8_1"
                                              :items="subjects8_1"
                                               item-title="text"
                                               item-value="value"
                                              label="八級(細目)"></v-select>
                                </v-col>
                            </v-row>
                            <v-row>
                               
                                <v-col cols="12" md="3">
                                    <v-text-field v-model="AllocateForm.Note"
                                                  label="摘要"
                                                  :rules="[rules.required]"></v-text-field>
                                </v-col>
                            </v-row>
                           
                        </v-card-text>
                        <v-card-actions>
                            <v-btn type="submit" color="primary">確認</v-btn>
                            <v-btn color="secondary"
                                   @click="cancelData">取消</v-btn>
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
import type { UserDataModel, AllocateFormViewModel, SelectedBudgetModel, UserViewModel } from '@/types/apiInterface';
import { get, put, post, type ApiResponse } from '@/services/api';
import type { VDataTable } from 'vuetify/components';
import { RULES } from '@/constants/constants';
type ReadonlyHeaders = VDataTable['$props']['headers'];

    const props = defineProps({
        data: {
            type: Object as () => SelectedBudgetModel,
            require: true
        },
    });
    console.log('props.data:', props.data);
    const sourceData: SelectedBudgetModel = props.data!;
const emit = defineEmits(['cancel']);
const AllocateFormRef = ref<HTMLFormElement | null>(null);
const loading = ref(false);
const groups = ref([
    "無",
    "工務組",
    "航務組",
    "企劃組",
    "總務組",
    "中控室",
    "業務組",
    "人事室",
    "政風室",
    "南竿站",
    "北竿站",
    "營運安全組",
    "企劃行政組"
]);
const subjects6 = ref<any[]>([{ text: '無', value: "0" }]);
const subjects7 = ref([{ text: '無', value: "0" }]);
const subjects8 = ref([{ text: '無', value: "0" }]);
const subjects6_1 = ref([{ text: '無', value: "0" }]);
const subjects7_1 = ref([{ text: '無', value: "0" }]);
const subjects8_1 = ref([{ text: '無', value: "0" }]);
    const people = ref<string[]>([]);
    // 取得當年度的民國年
    const currentYear: number = new Date().getFullYear() - 1911;
    // 生成從111到當年度的年份陣列
    const years = ref<number[]>(Array.from({ length: currentYear - 111 + 1 }, (_, i) => 111 + i)); 
   
    const defaultUser: UserViewModel = {
        No: 0,
        Name: '',
        Account: '',
        Password: '',
        Email: '',
        Unit_No: '',
        Auth: '',
        Account_Open: '',
        Reason: '',
        Count: 0,
        Time: new Date(),
        Time1: new Date(),
        Status1: '',
        Status2: '',
        MEMO: '',
        Status3: '',
    };

    const defaultAllocateForm: AllocateFormViewModel = {
        ID: 0,
        ID1: 0,
        Status: "O",
        Name: '',
        Group: '',
        Subject6: '',
        Subject7: '',
        Subject8: '',
        PurchaseMoney: 0,
        PayMoney: 0,
        Group1: '',
        Subject6_1: '',
        Subject7_1: '',
        Subject8_1: '',
        People: '',
        Year: currentYear, // 抓當年度
        Year1: currentYear.toString(),
        Purchasedate: '',
        Note: '',
        People1: '',
        Text: '',
        All: null,
        True: null,
    };

    const user = ref<UserViewModel>(defaultUser); 

    const AllocateForm = ref<AllocateFormViewModel>({
        ...defaultAllocateForm,
        Group: sourceData.Group!,
        Name: sourceData.Budget!,
        Subject6: sourceData.Subject6 ?? '',
        Subject7: sourceData.Subject7 ?? '',
        Subject8: sourceData.Subject8 ?? '',
        People: user.value.Name,
        People1: user.value.Name,
        Remarks: '',
        Text: "",
    });

    const rules = {
        ...RULES,
        lessThanOrEqualToBudget: (value: number) => {
            return value <= sourceData.UseBudget! || `請購金額不能大於 ${sourceData.UseBudget}`;
        },
        //lessThanOrEqualToPurchaseMoney: (value: number) => {
        //    return value <= limitPurchaseMoney.value || '實付金額不能大於請購金額';
        //}
    };
    
    //watch(() => AllocateForm.value.group, async () => {
    //    await fetchSubjects6();
    //});
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
            const response: ApiResponse<UserDataModel[]> = await get<UserDataModel[]>(url);  // 假設有一個 API 提供請購人資料
            //console.log('Data:', response.Data);
            if (response.StatusCode == 200) {
                people.value = response.Data?.map((person: UserDataModel) => person.Name ?? '') || [];
            }
        } catch (error) {
            console.error('Failed to fetch people:', error);
        }
    };

    const fetchSubjects6 = async () => {
        //console.log('fetchSubjects6 called');
        if (!AllocateForm.value.Group) return;
        const url = '/api/Type1/Subjects6';
        const data = { group: AllocateForm.value.Group };
        try {
            const response: ApiResponse<any> = await get<any>(url, data);
            if (response.StatusCode == 200) {
                //console.log(response.Data);
                // 將資料轉換成 { text: Name, value: ID } 的結構 concat方法可以展開陣列
                subjects6.value = [{ text: "無", value: "0" }].concat(
                    response.Data.map((item: { Name: string; ID: string }) => ({
                        text: item.Name,
                        value: item.ID,
                    }))
                );
                //console.log(subjects6.value);
            }
        } catch (error) {
            console.error('Failed to fetch subjects6:', error);
        }
    };

    const fetchSubjects7 = async () => {
        if (!AllocateForm.value.Subject6) return;
        const url = '/api/Type2/Subjects7';
        const data = {
            group: AllocateForm.value.Group,
            id: AllocateForm.value.Subject6
        };
        try {
            const response: ApiResponse<any> = await get<any>(url, data);
            if (response.StatusCode == 200) {
                subjects7.value = [{ text: "無", value: "0" }].concat(
                    response.Data.map((item: { Name: string; ID: string }) => ({
                        text: item.Name,
                        value: item.ID,
                    }))
                );
            }
        } catch (error) {
            console.error('Failed to fetch subjects7:', error);
        }
    };

    const fetchSubjects8 = async () => {
        if (!AllocateForm.value.Subject7) return;
        const url = '/api/Type3/Subjects8';
        const data = {
            group: AllocateForm.value.Group,
            id: AllocateForm.value.Subject7
        };
        try {
            const response: ApiResponse<any> = await get<any>(url, data);
            if (response.StatusCode == 200) {
                subjects8.value = [{ text: '無', value: '' }].concat(
                    response.Data.map((item: { Name: string, ID: string }) => ({
                    text: item.Name,
                    value: item.ID
                })));
            }
        } catch (error) {
            console.error('Failed to fetch subjects8:', error);
        }
    };

    const fetchSubjects6_1 = async () => {
        if (!AllocateForm.value.Group1) return;
        const url = '/api/Type1/Subjects6_1';
        const data = {
            group: AllocateForm.value.Group1,
            id: AllocateForm.value.Subject6.substring(0, 2) // 提取前兩個字元
        };
        try {
            const response: ApiResponse<any> = await get<any>(url, data);
            if (response.Data == '這個組室沒有指定科目!') {
                alert(response.Data);
                //AllocateForm.value = {...defaultAllocateForm };
                return;
            }
            if (response.StatusCode == 200) {
                subjects6_1.value = [{ text: "無", value: "0" }].concat(
                    response.Data?.map((item: { Name: string; ID: string }) => ({
                        text: item.Name,
                        value: item.ID,
                    }))
                );
            }
        } catch (error) {
            console.error('Failed to fetch subjects6_1:', error);
        }
    };

    const fetchSubjects7_1 = async () => {
        if (!AllocateForm.value.Subject6_1) return;
        const url = '/api/Type2/Subjects7';
        const data = {
            group: AllocateForm.value.Group1,
            id: AllocateForm.value.Subject6_1
        };
        try {
            const response: ApiResponse<any> = await get<any>(url, data);
            if (response.StatusCode == 200) {
                subjects7_1.value = [{ text: "無", value: "0" }].concat(
                    response.Data.map((item: { Name: string; ID: string }) => ({
                        text: item.Name,
                        value: item.ID,
                    }))
                );
            }
        } catch (error) {
            console.error('Failed to fetch subjects7_1:', error);
        }
    };

    const fetchSubjects8_1 = async () => {
        if (!AllocateForm.value.Subject7_1) return;
        const url = '/api/Type3/Subjects8';
        const data = {
            group: AllocateForm.value.Group1,
            id: AllocateForm.value.Subject7_1
        };
        try {
            const response: ApiResponse<any> = await get<any>(url, data);
            if (response.StatusCode == 200) {
                subjects8_1.value = [{ text: '無', value: '' }].concat(
                    response.Data.map((item: { Name: string, ID: string }) => ({
                        text: item.Name,
                        value: item.ID
                    })));
            }
        } catch (error) {
            console.error('Failed to fetch subjects8_1:', error);
        }
    };

    //const handleSubmit = async () => {
    //  /*  const url = '/api/BalanceManagement';*/
    //    const subject8Value = AllocateForm.value.Subject8;
    //    const subject8Text = subjects8.value.find(item => item.value === subject8Value)?.text;
    //    const subject7Text = subjects7.value.find(item => item.value === AllocateForm.value.Subject7)?.text;
    //    const data = {
    //        ...AllocateForm.value,
    //        //Budget: subject8Value === "" ? `${subject7Text?.slice(0, 4)}00${subject7Text?.slice(4)}` : subject8Text
    //    };
    //    try {
    //        console.log(data);
    //        //const response: ApiResponse<any> = await post<any>(url, data);
    //        //console.log(response.Data);
    //        //alert('Data submitted successfully');
    //    } catch (error) {
    //        console.error('Failed to submit data:', error);
    //        //alert('Failed to submit data');
    //    }
    //};



    const handleSubmit = async () => {

        const { valid } = await AllocateFormRef.value?.validate();
        if (!valid) return;

        const url = '/api/Money3/ID1';
        let ID1: number = 0;
        try {
            const response: ApiResponse<any> = await get<any>(url);
            if (response.StatusCode == 200) {
                ID1 = response.Data;
            }
        } catch (error) {
            console.error(error);
        }

        if (ID1 <= 0) {
            console.error('Invalid ID1:', ID1);
            return;
        }

        const dataOut = {
            ...AllocateForm.value,
            ID1: ID1, // 更新ID1屬性
            Text: "勻出", // 改成帶參數進去
            Remarks: AllocateForm.value.Group1 + `${AllocateForm.value.Subject7_1 + '00'}`,
            Group1: AllocateForm.value.Group,
            Year1: AllocateForm.value.Year.toString()
        };
        const dataIn = {
            ...dataOut,
            Text: "勻入",
            Group1: AllocateForm.value.Group1
        }
        saveMoney3(dataOut);
        saveMoney3(dataIn);
    };

    const saveMoney3 = async (AllocateFormVal: AllocateFormViewModel) => {
        const url = '/api/Money3';
        const data = AllocateFormVal;
        try {
            console.log(data);
            const response: ApiResponse<any> = await post<any>(url, data);
            console.log(response.Data);
            //alert('Data submitted successfully');
        } catch (error) {
            console.error('Failed to submit data:', error);

        }
    };


    //const handleSubmit = async () => {
    //    const data = {
    //        ...AllocateForm.value,
    //        Text: "勻出",
    //    };
    //    data.PurchaseMoney = -data.PurchaseMoney;

    //    saveMoney3(data);
    //};

    //const saveMoney3 = async (data) => {
    //    const url = '/api/Money3';
    //    const data = data;
    //    try {
    //        console.log(data);
    //        //const response: ApiResponse<any> = await post<any>(url, data);
    //        //console.log(response.Data);
    //        //alert('Data submitted successfully');e
    //    } catch (error) {
    //        console.error('Failed to submit data:', error);
    //        //alert('Failed to submit data');
    //    }
    //};

    const cancelData = () => {
        emit('cancel');
    }

    // 監聽 user.value 的變化並更新 AllocateForm.People
    watch(user, (newValue) => {
        if (newValue.Name) {
            AllocateForm.value.People = newValue.Name;
            AllocateForm.value.People1 = newValue.Name;
        }
    });

    onMounted(async () => {
        await getCurrentUser();
        await fetchPeople();
    });

</script>

<style scoped>
 
</style>