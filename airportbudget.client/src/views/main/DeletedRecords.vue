<template>
    <v-container class="no-margin" fluid>
        <v-row justify="start">
            <v-col cols="12" sm="8" md="6">
                <v-row>
                    <v-col cols="3">
                        <v-select label="查詢年度"
                                  :items="years"
                                  v-model="searchYear"
                                  style="width: 100%;">
                        </v-select>
                    </v-col>
                    <v-col cols="3">
                        <v-text-field label="輸入摘要"
                                      v-model="descriptionInput"
                                      style="width: 100%;" />
                    </v-col>
                    <v-col cols="3">
                        <v-btn @click="searchDeletedRecords"
                               color="primary"
                               class="mt-2"
                               size="large">
                            查詢
                        </v-btn>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>

        <v-pagination v-model="page"
                      :length="pageCount"
                      :total-visible="7"
                      color="primary"></v-pagination>
        <v-data-table :headers="headers"
                      :items="paginatedItems"
                      item-key="name"
                      items-per-page="12"
                      loading-text="讀取中請稍後..."
                      items-per-page-text="每頁筆數"
                      :loading="loading"
                      style="width: 100%;"
                      hide-default-footer>
            <template v-slot:item.Type="{ item }">
                {{ TypeMapping[item.Type] }}
            </template>
            <template v-slot:item.RequestDate="{ item }">
                {{ formatDate(item.RequestDate) }}
            </template>
            <template v-slot:item.PaymentDate="{ item }">
                {{ formatDate(item.PaymentDate) }}
            </template>
            <template v-slot:item.RequestAmount="{ item }">
                {{ formatNumber(item.RequestAmount) }}
            </template>
            <template v-slot:item.PaymentAmount="{ item }">
                {{ formatNumber(item.PaymentAmount) }}
            </template>
            <template v-slot:item.actions="{ item }">
                <v-btn @click="restoreData(item)"
                       color="primary">
                    還原
                </v-btn>
            </template>
        </v-data-table>

    </v-container>
</template>


<script setup lang="ts">
import axios from 'axios';
import { ref, computed, onMounted } from 'vue';
import type { BudgetAmountViewModel, SelectedDetail, SoftDeleteViewModel } from '@/types/apiInterface';
//import { AuthMapping, ReverseAuthMapping, StatusMapping, ReverseStatusMapping } from '@/utils/mappings'; // 對應狀態碼到中文
import { get, put, type ApiResponse } from '@/services/api';
import type { VDataTable } from 'vuetify/components';
import { formatDate, formatNumber } from '@/utils/functions';
import { TypeMapping } from '@/utils/mappings';
type ReadonlyHeaders = VDataTable['$props']['headers'];
  
    const loading = ref(false);
    // 取得當年度的民國年
    const currentYear: number = new Date().getFullYear() - 1911;
    // 生成從111到當年度的年份陣列
    const years = ref<number[]>(Array.from({ length: currentYear - 111 + 1 }, (_, i) => 111 + i)); 
    const searchYear = ref<number>(113);
    const descriptionInput = ref<string>('');
    const deletedRecords = ref<BudgetAmountViewModel[]>([]);
    const headers: ReadonlyHeaders = [
        { title: '請購日期', key: 'RequestDate' },
        { title: '類別', key: 'Type' },
        { title: '摘要', key: 'Description' },
        { title: '請購金額', key: 'RequestAmount' },
        { title: '支付日期', key: 'PaymentDate' },
        { title: '實付金額', key: 'PaymentAmount' },
        { title: '請購人', key: 'RequestPerson' },
        { title: '支付人', key: 'PaymentPerson' },
        { title: '備註', key: 'Remarks' },
        { title: '預算名稱', key: 'Budget.BudgetName' },
        { title: '組室別', key: 'Budget.Group.GroupName' },
        { title: '', key: 'actions', sortable: false },
    ];
const searchDeletedRecords = async () => {
    const url = 'api/BudgetAmount/ByDeletedRecords';
    const data: any = { CreatedYear: searchYear.value, Description: ''};
    if (descriptionInput) data.Description = descriptionInput.value;
    try {
        //console.log(123);
        //console.log(data);
        const response: ApiResponse<BudgetAmountViewModel[]> = await get<BudgetAmountViewModel[]>(url, data);
        //console.log(response.Message);
        if (response.StatusCode == 200) {
            deletedRecords.value = response?.Data ?? [];
            page.value = 1;
            //console.log(items.value);
        }
    }
    catch (error) {
        console.error(error);
        }
    };
    const restoreData = async (data: BudgetAmountViewModel) => {
        const isConfirmed = confirm('你確定要還原嗎？');
        const url = 'api/BudgetAmount/ByRestoreData'
        if (isConfirmed) {
            try {
                //console.log('data', data);
                const response: ApiResponse<any> = await put<BudgetAmountViewModel>(url, data);
                if (response.StatusCode == 200) { // 如果成功再叫一次資料
                    //console.log(response.Message);
                    await searchDeletedRecords();
                }
            }
            catch (error) {
                console.error(error);
            }
        }
    };

const page = ref(1);
const itemsPerPage = 12;
const pageCount = computed(() => Math.ceil(deletedRecords.value.length / itemsPerPage));
const paginatedItems = computed(() => {
  const start = (page.value - 1) * itemsPerPage;
  const end = start + itemsPerPage;
    return deletedRecords.value.slice(start, end);
});
</script>

<style scoped>
 
</style>