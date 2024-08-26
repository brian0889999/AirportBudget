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
                        <v-select label="請購人"
                                  :items="people"
                                  v-model="requestPersonInput"
                                  style="width: 100%;">
                        </v-select>
                    </v-col>
                    <v-col cols="3">
                        <v-select label="支付人"
                                  :items="people"
                                  v-model="paymentPersonInput"
                                  style="width: 100%;">
                        </v-select>
                    </v-col>

                </v-row>
            </v-col>
            <v-col cols="3">
                <v-btn text="查詢"
                       :loading="loading"
                       @click="searchDeletedRecords"
                       color="primary"
                       class="mt-2"
                       size="large">
                    <template v-slot:loader>
                        <v-progress-circular indeterminate></v-progress-circular>
                    </template>
                </v-btn>
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
                <v-btn text="還原"
                       :loading="loading"
                       @click="restoreData(item)"
                       color="primary">
                    <template v-slot:loader>
                        <v-progress-circular indeterminate></v-progress-circular>
                    </template>
                </v-btn>
            </template>
        </v-data-table>

    </v-container>
</template>


<script setup lang="ts">
import axios from 'axios';
import { ref, computed, onMounted, watch } from 'vue';
import type { BudgetAmountViewModel, SelectedDetail, UserViewModel } from '@/types/apiInterface';
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
    const requestPersonInput = ref<string>('');
    const paymentPersonInput = ref<string>('');
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

    const defaultUser: UserViewModel = {
        UserId: 0,
        Name: '',
        Account: '',
        Password: '',
        RolePermissionId: 0,
        GroupId: 0,
        Status: false,
        System: undefined,
        LastPasswordChangeDate: undefined,
        ErrCount: 0,
        ErrDate: new Date(1990, 0, 1),
        "Group": {
            "GroupId": 0,
            "GroupName": ""
        },
        "RolePermission": {
            "RolePermissionId": 0,
            "PermissionType": 0
        }
    };
    //const user = ref<UserViewModel>(defaultUser);
    const people = ref<string[]>([]);
    const getUser = async () => {
        const url = '/api/User';
        try {
            const response: ApiResponse<UserViewModel[]> = await get<UserViewModel[]>(url);
            if (response.StatusCode === 200) {
                //user.value = data ? data : defaultUser;
                //user.value = response.Data ?? [];
                people.value = ["無"].concat(response.Data?.map((person: UserViewModel) => person.Name ?? '') || []);
                //console.log(user.value);
            }
            else {
                console.error(response.Data ?? response.Message);
            }
        }
        catch (error: any) {
            console.error(error.message);
        }
    };
const searchDeletedRecords = async () => {
    const url = 'api/BudgetAmount/ByDeletedRecords';
    const data: any = { createdYear: searchYear.value, description: '', requestPerson: '', paymentPerson: ''};
    if (descriptionInput) data.description = descriptionInput.value;
    if (requestPersonInput && requestPersonInput.value != "無") data.requestPerson = requestPersonInput.value;
    if (paymentPersonInput && paymentPersonInput.value != "無") data.paymentPerson = paymentPersonInput.value;
    try {
        loading.value = true;
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
    finally {
        loading.value = false;
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

const page = ref<number>(1);
const itemsPerPage: number = 12;
const pageCount = computed(() => Math.ceil(deletedRecords.value.length / itemsPerPage));
const paginatedItems = computed(() => {
  const start: number = (page.value - 1) * itemsPerPage;
  const end: number = start + itemsPerPage;
    return deletedRecords.value.slice(start, end);
});


    //// 監聽 deletedRecords 的變化
    //watch(deletedRecords, (newVal: BudgetAmountViewModel[], oldVal: BudgetAmountViewModel[]) => {
    //    // 如果當前頁碼超出總頁數，則將頁碼重置為最後一頁
    //    if (page.value > pageCount.value) {
    //        page.value = pageCount.value;
    //    }
    //    // 如果資料縮減到空的情況，頁碼重置為1
    //    if (newVal.length === 0) {
    //        page.value = 1;
    //    }
    //});


    //// 監聽 pageCount 或 page 變化時，paginatedItems 會自動更新
    //watch([deletedRecords, page], () => {
    //    // paginatedItems 會根據最新的 deletedRecords 和 page 自動更新
    //});

onMounted(async () => {
    await getUser();
});

</script>

<style scoped>
 
</style>