<template>
    <v-container>
        <v-row>
            <v-col cols="12">
                <v-card>
                    <v-card-title>
                        <h2 class="headline">民航事業作業基金執行情形表</h2>
                    </v-card-title>
                    <v-card-text>
                        <v-row>
                            <v-col cols="12" sm="4">
                                <v-select v-model="selectedYearFund"
                                          :items="years" item-title="text" item-value="value"
                                          label="年度"></v-select>
                            </v-col>
                            <v-col cols="12" sm="4">
                                <v-select v-model="startMonthFund"
                                          :items="months" item-title="text" item-value="value"
                                          label="開始月份"></v-select>
                            </v-col>
                            <v-col cols="12" sm="4">
                                <v-select v-model="endMonthFund"
                                          :items="months" item-title="text" item-value="value"
                                          label="結束月份"></v-select>
                            </v-col>
                        </v-row>
                        <v-row>
                            <v-col cols="12">
                                <v-btn size="large" color="primary" @click="exportFundData">匯出</v-btn>
                            </v-col>
                        </v-row>
                    </v-card-text>
                </v-card>
            </v-col>
            <v-col cols="12">
                <v-form ref="exportBudgetExcelRef">
                    <v-card>
                        <v-card-title>
                            <h2 class="headline">預算控制執行情形表</h2>
                        </v-card-title>
                        <v-card-text>
                            <v-row>
                                <v-col cols="12" sm="4">
                                    <v-select v-model="selectedGroupIdBudget"
                                              :items="groups" item-title="text" item-value="value"
                                              label="組室"
                                              :rules="[rules.required]"></v-select>
                                </v-col>
                                <v-col cols="12" sm="4">
                                    <v-select v-model="selectedYearBudget"
                                              :items="years" item-title="text" item-value="value"
                                              label="年度"></v-select>
                                </v-col>
                                <v-col cols="12" sm="4">
                                    <v-select v-model="startMonthBudget"
                                              :items="months" item-title="text" item-value="value"
                                              label="開始月份"></v-select>
                                </v-col>
                                <v-col cols="12" sm="4">
                                    <v-select v-model="endMonthBudget"
                                              :items="months" item-title="text" item-value="value"
                                              label="結束月份"></v-select>
                                </v-col>
                            </v-row>
                            <v-row>
                                <v-col cols="12">
                                    <v-btn size="large" color="primary" @click="exportBudgetData">匯出</v-btn>
                                </v-col>
                            </v-row>
                        </v-card-text>
                    </v-card>
                </v-form>
            </v-col>
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
    import { ref, onMounted, watch } from 'vue';
    //import type { VDataTable } from 'vuetify/components';
    //import { useRoute } from 'vue-router';
    import { get, post, type ApiResponse } from '@/services/api';
    import { downloadFile } from '@/services/excelAPI'
    import type { SelectedOption } from '@/types/vueInterface';
    import type { ExportFundRequestViewModel, ExportBudgetRequestViewModel } from '@/types/apiInterface';
    import { RULES } from '@/constants/constants';

    const exportBudgetExcelRef = ref<HTMLFormElement | null>(null);
    const rules = RULES;
    const startYear: number = 111;
    const currentYear: number = new Date().getFullYear();
    const currentTaiwanYear: number = currentYear - 1911;
    const years: SelectedOption[] = Array.from({ length: currentTaiwanYear - startYear + 1 }, (_, i) => ({
        text: `${startYear + i}年`,
        value: startYear + i
    }));
    const months: SelectedOption[] = Array.from({ length: 12 }, (_, i) => ({
        text: `${i + 1}月`,
        value: i + 1
    }));

    // 民航事業作業基金執行情形表
    const selectedYearFund = ref<number>(currentTaiwanYear);
    const startMonthFund = ref<number>(months[0].value);
    const endMonthFund = ref<number>(months[11].value);

    // 預算控制執行情形表
    const selectedYearBudget = ref<number>(currentTaiwanYear);
    const startMonthBudget = ref<number>(months[0].value);
    const endMonthBudget = ref<number>(months[11].value);
    const selectedGroupIdBudget = ref<number>(0);
    const groups = ref<SelectedOption[]>([]);

    const getGroups = async () => {
        try {
            const url = '/api/Group/SelectedOption';
            const response: ApiResponse<SelectedOption[]> = await get<SelectedOption[]>(url);
            //console.log(1);
            if (response.StatusCode == 200) {
                //console.log(2);
                const data = response.Data;
                groups.value = data ?? [];
                groups.value.unshift({ text: '請選擇', value: 0 });
                //console.log(groups.value);
            }
        } catch (error) {
            console.error('Error downloading the file', error);
        }
    };

    const exportFundData = async () => {
        //console.log('Selected Year (Fund):', selectedYearFund.value);
        //console.log('Start Month (Fund):', startMonthFund.value);
        //console.log('End Month (Fund):', endMonthFund.value);
        // 實際的匯出邏輯在這裡實作
        try {
            const url = '/api/BudgetAmount/ExportFundExcel';
            const data: ExportFundRequestViewModel = {
                Year: selectedYearFund.value,
                StartMonth: startMonthFund.value,
                EndMonth: endMonthFund.value
            }
            const fileBlob = await downloadFile(url, data, 'params');

            //const blobUrl = window.URL.createObjectURL(new Blob([blob]));
            const blobUrl = window.URL.createObjectURL(fileBlob);
            const link = document.createElement('a');
            link.href = blobUrl;
            link.setAttribute('download', `工務組${data.Year}年${data.StartMonth}-${data.EndMonth}民航事業作業基金執行情形表.xlsx`); // 指定下載的檔案名
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (error) {
            console.error('Error downloading the file', error);
        }
    };



    const exportBudgetData = async () => {
        //console.log('Selected Group (Budget)', selectedGroupIdBudget);
        //console.log('Selected Year (Budget):', selectedYearBudget.value);
        //console.log('Start Month (Budget):', startMonthBudget.value);
        //console.log('End Month (Budget):', endMonthBudget.value);
        const { valid } = await exportBudgetExcelRef.value?.validate();
        if (!valid) return;
        // 實際的匯出邏輯在這裡實作
        try {
            const url = '/api/BudgetAmount/ExportBudgetExcel';
            const data: ExportBudgetRequestViewModel = {
                GroupId: selectedGroupIdBudget.value,
                Year: selectedYearBudget.value,
                StartMonth: startMonthBudget.value,
                EndMonth: endMonthBudget.value
            }
            const fileBlob = await downloadFile(url, data, 'params');

            //const blobUrl = window.URL.createObjectURL(new Blob([blob]));
            const blobUrl = window.URL.createObjectURL(fileBlob);
            const link = document.createElement('a');
            link.href = blobUrl;
            link.setAttribute('download', `預算控制執行情形表.xlsx`); // 指定下載的檔案名
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (error) {
            console.error('Error downloading the file', error);
        }
    };

    onMounted(async () => {
        await getGroups();
    });



</script>
<style scoped>
    .v-data-table__tr:hover {
        background-color: #232F34;
    }
</style>