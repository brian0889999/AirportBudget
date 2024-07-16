<template>
    <v-container class="no-margin" fluid>
        <v-row justify="start" v-if="!isSelectedItem && !showAllocatePage">
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
                        <v-select label="查詢組別"
                                  :items="filteredGroups"
                                  v-model="searchGroup"
                                  style="width: 100%;">
                        </v-select>
                    </v-col>
                    <v-col cols="3">
                        <v-btn @click="fetchBudgetData"
                               color="primary"
                               class="mt-2"
                               size="large">
                            查詢
                        </v-btn>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
        <v-data-table v-if="!isSelectedItem && !showAllocatePage"
                      :headers="headers"
                      :items="computedItems"
                      item-key="Budget"
                      items-per-page="12"
                      loading-text="讀取中請稍後..."
                      items-per-page-text="每頁筆數"
                      :loading="loading"
                      style="width: 100%;">
            <template #item.Budget="{ item }">
                <v-btn variant="flat"
                       class="mb-2 v-btn--text"
                       @click="handleBudgetClick(item)">
                    {{ item.Budget }}
                </v-btn>
                <br />
                <v-btn v-if="item.Budget" @click="handleExcelClick(item.Budget)"
                       color="primary">
                    EXCEL
                </v-btn>
            </template>
            <template #item.action="{ item }">
                <v-btn v-if="canAdd"
                       color="primary"
                       @click="openAllocatePage(item)">勻出</v-btn>
            </template>
            <template #item.BudgetYear="{ item }">
                <span :class="{'negative-number': (item.BudgetYear ?? 0) < 0}">
                    {{ formatNumber(item.BudgetYear) }}
                </span>
            </template>
            <template #item.Final="{ item }">
                <span :class="{'negative-number': parseFloat(item.Final ?? '0') < 0}">
                    {{ formatNumber(item.Final) }}
                </span>
            </template>
            <template #item.General="{ item }">
                <span :class="{'negative-number': (item.General ?? 0) < 0}">
                    {{ formatNumber(item.General) }}
                </span>
            </template>
            <template #item.Out="{ item }">
                <span :class="{'negative-number': (item.Out ?? 0) < 0}">
                    {{ formatNumber(item.Out) }}
                </span>
            </template>
            <template #item.UseBudget="{ item }">
                <span :class="{'negative-number': (item.UseBudget ?? 0) < 0}">
                    {{ formatNumber(item.UseBudget) }}
                </span>
            </template>
            <template #item.In="{ item }">
                <span :class="{'negative-number': (item.In ?? 0) < 0}">
                    {{ formatNumber(item.In) }}
                </span>
            </template>
            <template #item.InActual="{ item }">
                <span :class="{'negative-number': (item.InActual ?? 0) < 0}">
                    {{ formatNumber(item.InActual)}}
                </span>
            </template>
            <template #item.InBalance="{ item }">
                <span :class="{'negative-number': (item.InBalance ?? 0) < 0}">
                    {{ formatNumber(item.InBalance)}}
                </span>
            </template>
            <template #item.SubjectActual="{ item }">
                <span :class="{'negative-number': (item.SubjectActual ?? 0)<  0}">
                    {{ formatNumber(item.SubjectActual)}}
                </span>
            </template>
        </v-data-table>
         <!--isSelectedItem--> 
        <v-row justify="start" v-if="isSelectedItem && !showDetailForm && !showAllocatePage">
            <v-col cols="12" sm="8" md="6">
                <v-btn @click="previousPage"
                       color="primary"
                       class="mb-2">
                    回上一頁
                </v-btn>
            </v-col>
        </v-row>
        
        <v-data-table v-if="isSelectedItem && !showDetailForm && !showAllocatePage"
                      :headers="selectedHeaders"
                      :items="selectedItem"
                      hide-default-footer>
            <template #item.BudgetYear="{ item }">
                <span :class="{'negative-number': (item.BudgetYear ?? 0) < 0}">
                    {{ formatNumber(item.BudgetYear) }}
                </span>
            </template>
            <template #item.Final="{ item }">
                <span :class="{'negative-number': parseFloat(item.Final ?? '0') < 0}">
                    {{ formatNumber(item.Final) }}
                </span>
            </template>
            <template #item.General="{ item }">
                <span :class="{'negative-number': (item.General ?? 0) < 0}">
                    {{ formatNumber(item.General) }}
                </span>
            </template>
            <template #item.Out="{ item }">
                <span :class="{'negative-number': (item.Out ?? 0) < 0}">
                    {{ formatNumber(item.Out) }}
                </span>
            </template>
            <template #item.UseBudget="{ item }">
                <span :class="{'negative-number': (item.UseBudget ?? 0) < 0}">
                    {{ formatNumber(item.UseBudget) }}
                </span>
            </template>
            <template #item.In="{ item }">
                <span :class="{'negative-number': (item.In ?? 0)< 0}">
                    {{ formatNumber(item.In) }}
                </span>
            </template>
            <template #item.InActual="{ item }">
                <span :class="{'negative-number': (item.InActual ?? 0) < 0}">
                    {{ formatNumber(item.InActual)}}
                </span>
            </template>
            <template #item.InBalance="{ item }">
                <span :class="{'negative-number': (item.InBalance ?? 0) < 0}">
                    {{ formatNumber(item.InBalance)}}
                </span>
            </template>
            <template #item.SubjectActual="{ item }">
                <span :class="{'negative-number': (item.SubjectActual ?? 0)<  0}">
                    {{ formatNumber(item.SubjectActual)}}
                </span>
            </template> 
            <template #item.End="{ item }">
                <span :class="{'negative-number': (item.End ?? 0) < 0}">
                    {{ formatNumber(item.End)}}
                </span>
            </template>
        </v-data-table>
        <v-data-table v-if="isSelectedItem && !showDetailForm && !showAllocatePage"
                      :headers="selectedDetailHeaders"
                      :items="selectedDetailItem">
            <template #top>
                <search-fields v-if="isSelectedItem && !showDetailForm && !showAllocatePage"
                               @search="handleSearch"
                               class="mt-1" />
                <v-row no-gutters>
                    <v-col v-if="canAdd">
                        <v-btn color="primary" @click="addItem" v-if="canAdd">新增</v-btn>
                    </v-col>
                </v-row>
            </template>
            <template #item.PurchaseMoney="{ item }">
                <span :class="{'negative-number': (item.PurchaseMoney ?? 0) < 0}">
                    {{ formatNumber(item.PurchaseMoney)}}
                </span>
            </template>
            <template #item.PayMoney="{ item }">
                <span :class="{'negative-number': (item.PayMoney ?? 0) < 0}">
                    {{ formatNumber(item.PayMoney)}}
                </span>
            </template>
            <template #item.actions="{ item }">
                <v-btn icon size="small" class="mr-2" @click="editItem(item)" v-if="canEdit(item)">
                    <v-icon>mdi-pencil</v-icon>
                </v-btn>
                <v-btn icon="mdi-delete" size="small" @click="deleteItem(item)" v-if="canEdit(item)" />
            </template>
        </v-data-table>
        <detail-form v-if="showDetailForm"
                     :isEdit="isEdit"
                     :item="editingItem"
                     :limitBudget="limitBudget"
                     :searchGroup="searchGroup"
                     :user="user"
                     @update="handleUpdate"
                     @create="handleCreate"
                     @cancel="cancelEdit" />
        <AllocatePage v-if="showAllocatePage"
                      :data="allocateForm.data"
                      @cancel="cancelAllocatePage"/>
    </v-container>
</template>


<script setup lang="ts">
    import axios from 'axios';
    import { ref, computed, onMounted, watch } from 'vue';
    import { get, post, put, type ApiResponse } from '@/services/api';
    import type { VDataTable } from 'vuetify/components';
    import type { EditViewModel, SelectedBudgetModel, MoneyItem, MoneyRawData, SoftDeleteViewModel, Detail, SelectedDetail, UserViewModel } from '@/types/apiInterface';
    import { formatDate, sumByCondition, groupBy, formatNumber } from '@/utils/functions';
    import { AuthMapping } from '@/utils/mappings';
    import DetailForm from '@/components/modules/DetailForm.vue';
    import SearchFields from '@/components/modules/SearchFields.vue';
    import AllocatePage from '@/components/modules/AllocatePage.vue';
    
    const loading = ref<boolean>(false);
    type ReadonlyHeaders = VDataTable['$props']['headers'];

    const headers: ReadonlyHeaders = [
        /*{ title: '預算名稱', key: '', value: 'budget' },*/
        { title: '預算名稱', key: 'Budget' },
        { title: '', key: 'action'},
        { title: '組室別', key: 'Group' },
        { title: '科目(6級)', key: 'Subject6' },
        { title: '科目(7級)', key: 'Subject7' },
        { title: '科目(8級)', key: 'Subject8' },
        { title: '年度預算額度(1)', key: 'BudgetYear' },
        { title: '併決算書(2)', key: 'Final' },
        { title: '一般動支數(3)', key: 'General' },
        { title: '勻出數(4)', key: 'Out' },
        { title: '可用預算餘額(5)=(1)+(2)-(3)-(4)', key: 'UseBudget' },
        { title: '勻入數(6)', key: 'In' },
        { title: '勻入實付數(7)', key: 'InActual' },
        { title: '勻入數餘額(8)=(6)-(7)', key: 'InBalance' },
        { title: '本科目實付數(9)', key: 'SubjectActual' },
    ];

    const selectedHeaders: ReadonlyHeaders = [
        { title: '6級(科目)', key: 'Subject6' },
        { title: '7級(子目)', key: 'Subject7' },
        { title: '8級(細目)', key: 'Subject8' },
        { title: '年度預算額度(1)', key: 'BudgetYear' },
        { title: '併決算數額(2)', key: 'Final' },
        { title: '一般動支數(3)', key: 'General' },
        { title: '勻出數(4)', key: 'Out' },
        { title: '一般預算餘額(不含勻入)(5)=(1)+(2)-(3)-(4)', key: 'UseBudget' },
        { title: '勻入數額(6)累計(勻入)請購金額', key: 'In' },
        { title: '勻入實付數額(7)累計(勻入)實付金額', key: 'InActual' },
        { title: '勻入數餘額(8)=(6)-(7)', key: 'InBalance' },
        { title: '本科目實付數(9)累計(一般)及(勻入)實付金額', key: 'SubjectActual' },
        { title: '(含勻入)可用預算餘額(10)=(1)+(2)-(3)-(4)+(6)', key: 'End' }
    ];

    const selectedDetailHeaders: ReadonlyHeaders = [
        { title: '請購日期', key: 'FormattedPurchasedate' },
        { title: '類別', key: 'Text' },
        { title: '摘要', key: 'Note' },
        { title: '請購金額', key: 'PurchaseMoney' },
        { title: '支付日期', key: 'FormattedPayDate' },
        { title: '實付金額', key: 'PayMoney' },
        { title: '請購人', key: 'People' },
        { title: '支付人', key: 'People1' },
        { title: '備註', key: 'Remarks' },
        { title: '未稅', key: 'All' },
        { title: '已對帳', key: 'True' },
        { title: '', key: 'actions', sortable: false },
    ];

    // 取得當年度的民國年
    const currentYear: number = new Date().getFullYear() - 1911;
    // 生成從111到當年度的年份陣列
    const years = ref<number[]>(Array.from({ length: currentYear - 111 + 1 }, (_, i) => 111 + i));
    const searchYear = ref<number>(currentYear);

    const items = ref<MoneyItem[]>([]);
    const isSelectedItem = ref<boolean>(false);
    const selectedItem = ref<SelectedBudgetModel[]>([]);
    const selectedDetailItem = ref<Detail[]>([]);
    const groups = ref<string[]>(['工務組', '業務組', '人事室', '中控室', '北竿站', '企劃組', '南竿站', '政風室', '航務組', '總務組', '企劃行政組', '營運安全組']);
    const searchGroup = ref<string>('');
    const isEdit = ref(false);

    const currentBudgetValue = ref<string>(''); // 儲存 budgetValue 的變數
    const limitBudget = ref<number>(0); // 新增修改資料時的限制金額
    const showAllocatePage = ref<boolean>(false);

    const allocateForm = ref({
        visible: false,
        data: {},
    })

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

    const defaultMoneyRawData: MoneyRawData = { // 新增功能的初始值
        ID: 0,
        Purchasedate: '',
        Text: '一般',
        Note: '',
        PurchaseMoney: 0,
        PayDate: '',
        PayMoney: 0,
        People: '',
        Name: currentBudgetValue.value,
        Remarks: '',
        People1: '',
        ID1: 0,
        Status: 'O',
        Group1: searchGroup.value,
        Year: searchYear.value,
        Year1: '',
        All: '',
        True: '',
        Money: {
            ID: 0,
            Budget: '',
            Group: '',
            Subject6: '',
            Subject7: '',
            Subject8: '',
            BudgetYear: 0,
            Final: '',
            Year: 0,
        },
    };
    const user = ref<UserViewModel>(defaultUser); 

    const getCurrentUser = async () => {
    const url = '/api/User/Current';
    try {
        const response: ApiResponse<UserViewModel> = await get<UserViewModel>(url);
        if (response.StatusCode === 200) {
            const data = response.Data;
            user.value = data ? data : defaultUser;
            //console.log('user', user.value);
            searchGroup.value = AuthMapping[user.value.Auth!];
            defaultMoneyRawData.People = user.value.Name;
        }
        else {
            console.error(response.Data ?? response.Message);
        }
    }
    catch (error: any) {
        console.error(error.message);
    }
};
    const filteredGroups = computed(() => {
        if (user.value.Status1 === 'B') {
            return [searchGroup.value];
        }
        return groups.value;
    });

    const canAdd = computed(() => {
        if (user.value.Status1 === 'B') {
            return false;
        }
        else {
            return true;
        }
    });

    const canEdit = (item: Detail) => {
        //console.log('item: ',item);
        if (user.value.Status1 === 'A') {
            return true;
        }
        else if (user.value.Status1 === 'B') {
            return false;
        }
        else if (user.value.Status1 === 'C' && item.True != "V") {
            return item.People === user.value.Name;  // 權限是C時資料的請購人與使用者相同時才能編輯資料,已對帳的只有A權限可以編輯
        }
        return false;
    };

    const previousPage = async () => {
        isSelectedItem.value = false;
        try {
            await fetchBudgetData(); // 重新取一次資料(不管有沒有更新或刪除)
        }
        catch (error) {
            console.error(error);
        };
    };
  
    const fetchBudgetData = async () => {
        const url = '/api/Money3';
        //const data = { params: { Year: searchYear.value } }; 
        const data = { Year: searchYear.value, Group: searchGroup.value };  // 抓年度的值
        loading.value = true;
        try {
            const response: ApiResponse<MoneyRawData[]> = await get<MoneyRawData[]>(url, data);
            if (response.StatusCode === 200) {
                //console.log('data:', response.Data);
                //console.log(response.StatusCode);
                const dbData = response.Data?.map((item: MoneyRawData): MoneyItem => ({
                    Budget: item.Money.Budget,
                    Group: item.Money.Group,
                    Subject6: item.Money.Subject6,
                    Subject7: item.Money.Subject7,
                    Subject8: item.Money.Subject8,
                    BudgetYear: item.Money.BudgetYear,
                    Final: item.Money.Final,
                    Text: item.Text,
                    PurchaseMoney: item.PurchaseMoney,
                    PayMoney: item.PayMoney,
                    Purchasedate: item.Purchasedate || ""  // 確保不為 undefined
                })) ?? [];
                items.value = dbData;
                //items.value = response.Data ?? [];
                //console.log('items', items.value);
            } else {
                console.log(response.Data ?? response.Message)
            }
        }
        catch (error: any) {
            console.error(error)
        }
        finally {
            loading.value = false;
            //console.log('end');
        }
    };

    const fetchSelectedDetail = async (Budget: string, Group: string, Year: number, Note?: string, PurchaseMoney?: number) => {
        const url = '/api/Money3/SelectedDetail';
        const data: any = { Budget, Group, Year };

        if (Note) data.Note = Note;
        if (PurchaseMoney) data.PurchaseMoney = PurchaseMoney;
        //console.log(data);
        try {
            loading.value = true;
            const response: ApiResponse<MoneyRawData[]> = await get<MoneyRawData[]>(url, data);
            if (response.StatusCode === 200) {
                //filteredDetailItem.value = response.Data!;
                selectedDetailItem.value = response.Data?.map(item => ({
                    ...item,
                    All: item.All?.trim(),
                    True: item.True?.trim(),
                    FormattedPurchasedate: formatDate(item.Purchasedate || ""),
                    FormattedPayDate: formatDate(item.PayDate || "")
                })).sort((a, b) => new Date(b.Purchasedate || "").getTime() - new Date(a.Purchasedate || "").getTime()) ?? [];
                //console.log('selectedDetailItem', selectedDetailItem.value);
            } else {
                console.error(response.Message);
            }
        } catch (error) {
            console.error(error);
        } finally {
            loading.value = false;
        }
    };

    // sumByCondition透過condition(篩選條件是item的Text欄位)找出那一欄位的field做總和
    // groupBy用於分組 用下面7個欄位groupBy
    const computedItems = computed(() => {  
        const groupedItems = groupBy(items.value, ['Budget', 'Group', 'Subject6', 'Subject7', 'Subject8', 'BudgetYear', 'Final']);
        const sortedItems = Object.values(groupedItems).map(group => {
            const firstItem = group[0];
            const general = sumByCondition(group, '一般', 'PurchaseMoney');
            const out = sumByCondition(group, '勻出', 'PurchaseMoney');
            const inValue = sumByCondition(group, '勻入', 'PurchaseMoney');
            const inActual = sumByCondition(group, '勻入', 'PayMoney');
            const subjectActual = inActual + sumByCondition(group, '一般', 'PayMoney');

            // 將 Final 欄位從字串轉換成數字
            const finalValue = parseFloat(firstItem.Final ?? "") || 0;
            const budgetYear = firstItem.BudgetYear || 0;

            const useBudget = budgetYear + finalValue - out - general;
            const end = budgetYear + finalValue - general - out + inValue;
            const inBalance = inValue - inActual;

            return {
                ...firstItem,
                General: general,
                Out: out,
                In: inValue,
                InActual: inActual,
                UseBudget: useBudget,
                End: end,
                InBalance: inBalance,
                SubjectActual: subjectActual
            };
        });
        //按照日期排序
        //return sortedItems.sort((a, b) => new Date(b.Purchasedate || "").getTime() - new Date(a.Purchasedate || "").getTime());
        return sortedItems;
    });

    const handleBudgetClick = async (budget: SelectedBudgetModel) => {
        isSelectedItem.value = true;
        const data = [{ ...budget }]; // 將整個item複製
        //console.log('Budget clicked:', data);
        selectedItem.value = data.map((v) => {
            const newData: SelectedBudgetModel = {
                Subject6: v.Subject6,
                Subject7: v.Subject7,
                Subject8: v.Subject8,
                BudgetYear: v.BudgetYear || 0,
                Final: v.Final || "0",
                General: v.General,
                Out: v.Out,
                UseBudget: v.UseBudget,
                In: v.In,
                InActual: v.InActual,
                InBalance: v.InBalance,
                SubjectActual: v.SubjectActual,
                End: v.End
            };
            return newData;
        });
        // 篩選 selectedDetailItem，選出 Budget 欄位與傳進來的 Budget 欄位值相同的資料   
        budget.Budget ? currentBudgetValue.value = budget.Budget  : ''; // 儲存 budgetValue
        await fetchSelectedDetail(budget.Budget!, searchGroup.value, searchYear.value);
    };

    const handleExcelClick = async (budget: string) => {
        try {
            const response = await axios.get(`/api/Money3/ExportToExcel?budget=${budget}`, {
                responseType: 'blob'
            });
            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `${budget}.xlsx`);
            document.body.appendChild(link);
            link.click();
        } catch (error) {
            console.error(error);
        }
    };


    const showDetailForm = ref<boolean>(false);
    const editingItem = ref<MoneyRawData>(defaultMoneyRawData);

    const editItem = async (item: Detail) => {
        showDetailForm.value = true;
        isEdit.value = true;
        // 解構賦值，排除 FormattedPurchasedate 和 FormattedPayDate 欄位
        //console.log('console.log(item);', item);
        const { FormattedPurchasedate, FormattedPayDate, ...filteredItem } = item;
        editingItem.value = { ...filteredItem };
        limitBudget.value = selectedItem.value[0].UseBudget ?? 0;
        //console.log(item);
        //console.log('Edit item:', editingItem.value);
    };

    const handleUpdate = async (updatedItem: MoneyRawData) => {
        // 編輯項目的處理邏輯
        //console.log('updatedItem', updatedItem);
        try {
            //await requery(updatedItem); // 把資訊帶進去重查
            await fetchBudgetData();
            await fetchSelectedDetail(currentBudgetValue.value!, searchGroup.value, searchYear.value);
            // 重新更新 selectedItem
            const budgetItem = computedItems.value.find(budget => budget.Budget === currentBudgetValue.value);
            if (budgetItem) {
                await handleBudgetClick(budgetItem);
            }
        }
        catch (error) {
            console.error(error);
        };
    };
  
    const addItem = () => {
        showDetailForm.value = true;
        isEdit.value = false;
        limitBudget.value = selectedItem.value[0].UseBudget ?? 0;
        editingItem.value = defaultMoneyRawData;
    };

    const handleCreate = async (newItem: any) => {
        // 處理新增邏輯
        await fetchBudgetData();
        await fetchSelectedDetail(currentBudgetValue.value!, searchGroup.value, searchYear.value);
        // 重新更新 selectedItem
        const budgetItem = computedItems.value.find(budget => budget.Budget === currentBudgetValue.value);
        if (budgetItem) {
            await handleBudgetClick(budgetItem);
        }
        console.log('Created:', newItem);
    };

    const cancelEdit = () => {
        showDetailForm.value = false;
        editingItem.value = defaultMoneyRawData;
    };

    const deleteItem = async (item: MoneyRawData) => {
        // 刪除項目的處理邏輯
        const isConfirmed = confirm('你確定要刪除此項目嗎？');
        if (isConfirmed) {
            /*console.log('Delete item:', item);*/
            try {
                const url = '/api/Money3/SoftDelete';
                const response: ApiResponse<any> = await put<any>(url, item);
                if (response.StatusCode == 200) {
                    //console.log(response.Message);
                    await fetchBudgetData();
                    await fetchSelectedDetail(currentBudgetValue.value!, searchGroup.value, searchYear.value);
                    // 重新更新 selectedItem
                    const budgetItem = computedItems.value.find(budget => budget.Budget === currentBudgetValue.value);
                    if (budgetItem) {
                        await handleBudgetClick(budgetItem);
                    }
                }
            }
            catch (error) {
                console.error(error);
            }
        } else {
            // 如果使用者取消，則不進行任何操作
            console.log('取消刪除');
        }
    };


    const handleSearch = async (payload: { Note: string, PurchaseMoney: number }) => {
        // 處理查詢邏輯
        //console.log('Note:', payload.Note, 'Number:', payload.PurchaseMoney);
        await fetchSelectedDetail(currentBudgetValue.value!, searchGroup.value, searchYear.value, payload.Note, payload.PurchaseMoney);
    };

    const openAllocatePage = (item: SelectedBudgetModel) => {
        allocateForm.value.data = item;
        showAllocatePage.value = true;
    }

    const cancelAllocatePage = () => {
        showAllocatePage.value = false;
    }

     onMounted(async () => {
         await getCurrentUser();
        /* console.log('Mounted user status:', user.value.Status1); // 增加 log*/
     });

    watch(() => user.value, (newStatus) => {
        //console.log('Status changed:', newStatus);
    });
</script>

<style scoped>
    .no-margin {
        margin: 0;
    }
    /*.link-style {
        color: #007bff;
        text-decoration: underline;
        cursor: pointer;
    }*/

    .v-btn--text {
        color: blue;
        text-decoration: underline;
        background-color: transparent;
        border: none;
        padding: 0;
    }

    .v-btn--text:hover {
        color: darkblue;
    }
    .negative-number {
        color: red;
    }
</style>