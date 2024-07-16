<!--<template>
    <v-container>
        <v-form ref="balanceFormRef" @submit.prevent="handleSubmit">
            <v-row>
                <v-col cols="12">
                    <v-card outlined>
                        <v-card-title>經費管理</v-card-title>
                        <v-card-subtitle>從哪裡勻出</v-card-subtitle>
                        <v-card-text>
                            <v-row>
                                <v-col cols="12" md="3">
                                    <v-select v-model="balanceForm.Group"
                                              :items="groups"
                                              label="組室別"
                                              @update:modelValue="fetchSubjects6"
                                              required></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="balanceForm.Subject6"
                                              :items="subjects6"
                                              item-title="text"
                                              item-value="value"
                                              label="六級(科目)"
                                              @update:modelValue="fetchSubjects7"
                                              required></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="balanceForm.Subject7"
                                              :items="subjects7"
                                              item-title="text"
                                              item-value="value"
                                              label="七級(子目)"
                                              @update:modelValue="fetchSubjects8"
                                              required></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="balanceForm.Subject8"
                                              :items="subjects8"
                                              item-title="text"
                                              item-value="value"
                                              label="八級(細目)"
                                              required></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-text-field v-model="balanceForm.PurchaseMoney"
                                                  label="金額"
                                                  type="number"
                                                  required></v-text-field>
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
                                    <v-select v-model="balanceForm.Group1"
                                              :items="groups"
                                              label="組室別"
                                              @update:modelValue="fetchSubjects6_1"
                                              required></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="balanceForm.Subject6_1"
                                              :items="subjects6_1"
                                              item-title="text"
                                              item-value="value"
                                              label="六級(科目)"
                                              @update:modelValue="fetchSubjects7_1"
                                              required></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="balanceForm.Subject7_1"
                                              :items="subjects7_1"
                                               item-title="text"
                                               item-value="value"
                                              label="七級(子目)"
                                              @update:modelValue="fetchSubjects8_1"
                                              required></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="balanceForm.Subject8_1"
                                              :items="subjects8_1"
                                               item-title="text"
                                               item-value="value"
                                              label="八級(細目)"
                                              required></v-select>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="3">
                                    <v-select v-model="balanceForm.People"
                                              :items="people"
                                              label="請購人"
                                              required></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-select v-model="balanceForm.Year"
                                              :items="years"
                                              label="年度"
                                              required></v-select>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-text-field v-model="balanceForm.PurchaseDate"
                                                  label="請購日期"
                                                  type="date"
                                                  required></v-text-field>
                                </v-col>
                                <v-col cols="12" md="3">
                                    <v-text-field v-model="balanceForm.Note"
                                                  label="摘要"
                                                  required></v-text-field>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12" md="3">
                                    <v-select v-model="balanceForm.People1"
                                              :items="people"
                                              label="支付人"
                                              required></v-select>
                                </v-col>
                            </v-row>
                        </v-card-text>
                        <v-card-actions>
                            <v-btn type="submit" color="primary">確認</v-btn>
                            <v-btn color="secondary">取消</v-btn>
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
import type { UserDataModel, AllocateFormViewModel } from '@/types/apiInterface';
import { get, put, post, type ApiResponse } from '@/services/api';
import type { VDataTable } from 'vuetify/components';
type ReadonlyHeaders = VDataTable['$props']['headers'];


const balanceFormRef = ref<HTMLFormElement | null>(null);
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
    const defaultBalanceForm: AllocateFormViewModel = {
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
        PurchaseDate: '',
        Note: '',
        People1: '',
        Budget: ''
    };
    const balanceForm = ref<AllocateFormViewModel>({ ...defaultBalanceForm });

    onMounted(async () => {
        await fetchPeople();
    });

    //watch(() => balanceForm.value.group, async () => {
    //    await fetchSubjects6();
    //});

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
        if (!balanceForm.value.Group) return;
        const url = '/api/Type1/Subjects6';
        const data = { group: balanceForm.value.Group };
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
        if (!balanceForm.value.Subject6) return;
        const url = '/api/Type2/Subjects7';
        const data = {
            group: balanceForm.value.Group,
            id: balanceForm.value.Subject6
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
        if (!balanceForm.value.Subject7) return;
        const url = '/api/Type3/Subjects8';
        const data = {
            group: balanceForm.value.Group,
            id: balanceForm.value.Subject7
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
        if (!balanceForm.value.Group1) return;
        const url = '/api/Type1/Subjects6_1';
        const data = {
            group: balanceForm.value.Group1,
            id: balanceForm.value.Subject6
        };
        try {
            const response: ApiResponse<any> = await get<any>(url, data);
            if (response.Data == '這個組室沒有指定科目!') {
                alert(response.Data);
                //balanceForm.value = {...defaultBalanceForm };
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
        if (!balanceForm.value.Subject6_1) return;
        const url = '/api/Type2/Subjects7';
        const data = {
            group: balanceForm.value.Group1,
            id: balanceForm.value.Subject6_1
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
        if (!balanceForm.value.Subject7_1) return;
        const url = '/api/Type3/Subjects8';
        const data = {
            group: balanceForm.value.Group1,
            id: balanceForm.value.Subject7_1
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

    const handleSubmit = async () => {
        const url = '/api/BalanceManagement';
        const subject8Value = balanceForm.value.Subject8;
        const subject8Text = subjects8.value.find(item => item.value === subject8Value)?.text;
        const subject7Text = subjects7.value.find(item => item.value === balanceForm.value.Subject7)?.text;
        const data = {
            ...balanceForm.value,
            Budget: subject8Value === "" ? `${subject7Text?.slice(0, 4)}00${subject7Text?.slice(4)}` : subject8Text
        };
        try {
            console.log(data);
            const response: ApiResponse<any> = await post<any>(url, data);
            console.log(response.Data);
            //alert('Data submitted successfully');
        } catch (error) {
            console.error('Failed to submit data:', error);
            //alert('Failed to submit data');
        }
    };
</script>

<style scoped>
 
</style>-->