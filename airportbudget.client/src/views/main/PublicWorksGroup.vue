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
                                  :items="filteredGroups" item-title="text" item-value="value"
                                  v-model="searchGroup"
                                  style="width: 100%;">
                        </v-select>
                    </v-col>
                    <v-col cols="3">
                        <v-btn text="查詢"
                               :loading="loading"
                               @click="fetchBudgetData"
                               color="primary"
                               class="mt-2"
                               size="large">
                            <template v-slot:loader>
                                <v-progress-circular indeterminate></v-progress-circular>
                            </template>
                        </v-btn>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
        <v-data-table v-if="!isSelectedItem && !showAllocatePage"
                      :headers="headers"
                      :items="paginatedItems"
                      item-key="Budget"
                      items-per-page="12"
                      loading-text="讀取中請稍後..."
                      items-per-page-text="每頁筆數"
                      :loading="loading"
                      style="width: 100%;"
                      hide-default-footer>
            <!--這邊items-per-page要跟itemsPerPage一樣-->
            <template v-slot:top>
                <v-pagination v-model="page"
                              :length="pageCount"
                              class="mb-4"></v-pagination>
            </template>
            <template #item.action="{ item }">
                <v-btn text="內容"
                       :loading="loading"
                       color="primary"
                       class="mb-2"
                       @click="handleBudgetClick(item)">
                    <template v-slot:loader>
                        <v-progress-circular indeterminate></v-progress-circular>
                    </template>
                </v-btn>
                <v-btn text="勻出"
                       :loading="loading"
                       v-if="canAllocate"
                       color="primary"
                       @click="openAllocatePage(item)"
                       class="mb-2">
                    <template v-slot:loader>
                        <v-progress-circular indeterminate></v-progress-circular>
                    </template>
                </v-btn>
                <br />
                <v-btn text="EXCEL"
                       :loading="loading"
                       v-if="item.BudgetName"
                       class="mb-2"
                       @click="handleExcelClick(item)"
                       color="primary">
                    <template v-slot:loader>
                        <v-progress-circular indeterminate></v-progress-circular>
                    </template>
                </v-btn>
            </template>
            <!--<template #item.action="{ item }">
                <v-btn v-if="canAdd"
                       color="primary"
                       @click="openAllocatePage(item)">勻出</v-btn>
            </template>-->
            <template #item.AnnualBudgetAmount="{ item }">
                <span :class="{'negative-number': (item.AnnualBudgetAmount ?? 0) < 0}">
                    {{ formatNumber(item.AnnualBudgetAmount) }}
                </span>
            </template>
            <template #item.FinalBudgetAmount="{ item }">
                <span :class="{'negative-number': parseFloat(item.FinalBudgetAmount ?? '0') < 0}">
                    {{ formatNumber(item.FinalBudgetAmount) }}
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
                <v-btn text="回上一頁"
                       :loading="loading"
                       @click="previousPage"
                       color="primary"
                       class="mb-2">
                <template v-slot:loader>
                    <v-progress-circular indeterminate></v-progress-circular>
                </template>    
                </v-btn>
            </v-col>
        </v-row>

        <v-data-table v-if="isSelectedItem && !showDetailForm && !showAllocatePage"
                      :headers="selectedHeaders"
                      :items="selectedItem"
                      hide-default-footer>
            <template #item.AnnualBudgetAmount="{ item }">
                <span :class="{'negative-number': (item.AnnualBudgetAmount ?? 0) < 0}">
                    {{ formatNumber(item.AnnualBudgetAmount) }}
                </span>
            </template>
            <template #item.FinalBudgetAmount="{ item }">
                <span :class="{'negative-number': parseFloat(item.FinalBudgetAmount ?? '0') < 0}">
                    {{ formatNumber(item.FinalBudgetAmount) }}
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
                      :items="detailPaginatedItems"
                      hide-default-footer>
            <template #top>
                <search-fields v-if="isSelectedItem && !showDetailForm && !showAllocatePage"
                               :loading="loading"
                               @search="handleSearch"
                               class="mt-1" />
                <v-row no-gutters>
                    <v-col v-if="canAdd">
                        <v-btn text="新增"
                               :loading="loading"
                               color="primary"
                               @click="addItem" 
                               v-if="canAdd">
                            <template #loader>
                                <v-progress-circular indeterminate></v-progress-circular>
                            </template>
                        </v-btn>
                    </v-col>
                </v-row>
                <v-pagination v-model="detailPage"
                              :length="detailPageCount"
                              class="mb-4"></v-pagination>
            </template>
            <template #item.Type="{ item }">
                {{ TypeMapping[item.Type] }}
            </template>
            <template #item.RequestAmount="{ item }">
                <span :class="{'negative-number': (item.RequestAmount ?? 0) < 0}">
                    {{ formatNumber(item.RequestAmount)}}
                </span>
            </template>
            <template #item.PaymentAmount="{ item }">
                <span :class="{'negative-number': (item.PaymentAmount ?? 0) < 0}">
                    {{ formatNumber(item.PaymentAmount)}}
                </span>
            </template>
            <template #item.ExTax="{ item }">
                {{ formatBool(item.ExTax) }}
            </template>
            <template #item.Reconciled="{ item }">
                {{ formatBool(item.Reconciled) }}
            </template>
            <template #item.actions="{ item }">
                <v-btn icon size="small" class="mr-2" @click="editItem(item)" v-if="canEdit(item)">
                    <v-icon>mdi-pencil</v-icon>
                </v-btn>
                <v-btn icon="mdi-delete" class="mr-2" size="small" @click="deleteItem(item)" v-if="canEdit(item)" color="red-darken-1" />
                <!--<v-btn v-if="item.Type != 1" color="primary" @click="linkBudgetData(item)">{{ linkBudgetDataBtn(item) }}</v-btn>-->
                <v-btn icon="mdi-cable-data" v-if="item.LinkedBudgetAmountId" size="small" @click="linkBudgetData(item)" color="black" />
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
                      :searchYear="searchYear"
                      @cancel="cancelAllocatePage" />
    </v-container>


    <v-dialog v-model="linkBudgetForm.visible"
              max-width="1200px"
              @click:outside="cancelLinkBudgetForm">
        <LinkBudgetForm :data="linkBudgetForm.data"
                        @cancel="cancelLinkBudgetForm" />
    </v-dialog>
</template>


<script setup lang="ts">
    import axios from 'axios';
    import { ref, computed, onMounted, watch } from 'vue';
    import { get, post, put, type ApiResponse } from '@/services/api';
    import { downloadFile, postDataAndDownloadFile } from '@/services/excelAPI';
    import type { VDataTable } from 'vuetify/components';
    import type { BudgetAmountViewModel, Budget, SelectedBudgetModel, BudgetAmountExcelViewModel, SelectedDetail, UserViewModel } from '@/types/apiInterface';
    import type { SelectedOption } from '@/types/vueInterface';
    import { formatDate, sumByCondition, groupBy, formatNumber, formatBool } from '@/utils/functions';
    import { AuthMapping, TypeMapping } from '@/utils/mappings';
    import DetailForm from '@/components/modules/DetailForm.vue';
    import SearchFields from '@/components/modules/SearchFields.vue';
    import AllocatePage from '@/components/modules/AllocatePage.vue';
    import LinkBudgetForm from '@/components/modules/LinkBudgetForm.vue';
    
    const loading = ref<boolean>(false);
    type ReadonlyHeaders = VDataTable['$props']['headers'];

    const headers: ReadonlyHeaders = [
        /*{ title: '預算名稱', key: '', value: 'budget' },*/
        { title: '', key: 'action' },
        { title: '預算名稱', key: 'BudgetName', width: '6%' },
        { title: '組室別', key: 'GroupName', width: '6%' },
        { title: '科目(6級)', key: 'Subject6' },
        { title: '科目(7級)', key: 'Subject7' },
        { title: '科目(8級)', key: 'Subject8', width: '6%' },
        { title: '年度預算額度(1)', key: 'AnnualBudgetAmount', align: 'end', sortable: true, },
        { title: '併決算書(2)', key: 'FinalBudgetAmount', align: 'end' },
        { title: '一般動支數(3)', key: 'General', align: 'end' },
        { title: '勻出數(4)', key: 'Out', align: 'end' },
        { title: '可用預算餘額(5)=(1)+(2)-(3)-(4)', key: 'UseBudget', align: 'end' },
        { title: '勻入數(6)', key: 'In', align: 'end' },
        { title: '勻入實付數(7)', key: 'InActual', align: 'end' },
        { title: '勻入數餘額(8)=(6)-(7)', key: 'InBalance', align: 'end' },
        { title: '本科目實付數(9)', key: 'SubjectActual', align: 'end' },
    ];

    const selectedHeaders: ReadonlyHeaders = [
        { title: '6級(科目)', key: 'Subject6' },
        { title: '7級(子目)', key: 'Subject7' },
        { title: '8級(細目)', key: 'Subject8' },
        { title: '年度預算額度(1)', key: 'AnnualBudgetAmount', align: 'end' },
        { title: '併決算數額(2)', key: 'FinalBudgetAmount', align: 'end' },
        { title: '一般動支數(3)', key: 'General', align: 'end' },
        { title: '勻出數(4)', key: 'Out', align: 'end' },
        { title: '一般預算餘額(不含勻入)(5)=(1)+(2)-(3)-(4)', key: 'UseBudget', align: 'end' },
        { title: '勻入數額(6)累計(勻入)請購金額', key: 'In', align: 'end' },
        { title: '勻入實付數額(7)累計(勻入)實付金額', key: 'InActual', align: 'end' },
        { title: '勻入數餘額(8)=(6)-(7)', key: 'InBalance', align: 'end' },
        { title: '本科目實付數(9)累計(一般)及(勻入)實付金額', key: 'SubjectActual', align: 'end' },
        { title: '(含勻入)可用預算餘額(10)=(1)+(2)-(3)-(4)+(6)', key: 'End', align: 'end' }
    ];

    const selectedDetailHeaders: ReadonlyHeaders = [
        { title: '請購日期', key: 'FormattedRequestDate' },
        { title: '類別', key: 'Type' },
        { title: '摘要', key: 'Description', width: '8%' },
        { title: '請購金額', key: 'RequestAmount', align: 'end' },
        { title: '支付日期', key: 'FormattedPaymentDate' },
        { title: '實付金額', key: 'PaymentAmount', align: 'end' },
        { title: '請購人', key: 'RequestPerson' },
        { title: '支付人', key: 'PaymentPerson' },
        { title: '備註', key: 'Remarks' },
        { title: '未稅', key: 'ExTax' },
        { title: '已對帳', key: 'Reconciled' },
        { title: '', key: 'actions', sortable: false },
    ];

    // 取得當年度的民國年
    const currentYear: number = new Date().getFullYear() - 1911;
    // 生成從111到當年度的年份陣列
    const years = ref<number[]>(Array.from({ length: currentYear - 111 + 1 }, (_, i) => 111 + i));
    const searchYear = ref<number>(currentYear);

    const items = ref<Budget[]>([]);
    const isSelectedItem = ref<boolean>(false);
    const selectedItem = ref<SelectedBudgetModel[]>([]);
    const selectedDetailItem = ref<SelectedDetail[]>([]);
    const groups = ref<SelectedOption[]>([
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
    const searchGroup = ref<number>(0);
    const isEdit = ref(false);

    const currentBudgetId = ref<number>(0); 
    const currentBudgetName = ref<string>(''); // 儲存 budgetValue 的變數
    const limitBudget = ref<number>(0); // 新增修改資料時的限制金額
    const showAllocatePage = ref<boolean>(false);
    const linkBudgetDataBtn = (item: any) => item.Type == 2 ? '勻入資料' : '勻出資料'; 

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

    const defaultBudgetAmount: BudgetAmountViewModel = { // 新增功能的初始值
        BudgetAmountId: 0,
        Description: '',
        Type: 1,
        RequestAmount: 0,
        PaymentAmount: 0,
        RequestDate: null,
        RequestPerson: '',
        PaymentPerson: '',
        ExTax: false,
        Reconciled: false,
        Remarks: '',
        Status: 'O',
        CreatedYear: 0,
        AmountYear: 0,
        BudgetId: 0,
        //AmountSerialNumber: 0,
        IsValid: true,
        //Budget: {
        //    BudgetId: 0,
        //    BudgetName: '',
        //    Subject6: '',
        //    Subject7: '',
        //    Subject8: '',
        //    AnnualBudgetAmount: 0,
        //    FinalBudgetAmount: 0,
        //    CreatedYear: 0,
        //    GroupId: 0,
        //    Group: {
        //        GroupId: 0,
        //        GroupName: ''
        //    }
        //}
    };

    const defaultSelectedBudget: SelectedBudgetModel = {
        BudgetId: 0,
        BudgetName: '',
        GroupId: 0,
        GroupName: '',
        Subject6: '',
        Subject7: '',
        Subject8: '',
        AnnualBudgetAmount: 0,
        FinalBudgetAmount: 0,
        General: 0,
        Out: 0,
        UseBudget: 0,
        In: 0,
        InActual: 0,
        InBalance: 0,
        SubjectActual: 0,
        End: 0,
        Type: 0,
        RequestAmount: 0,
        PaymentAmount: 0
    };
    const allocateForm = ref({
        visible: false,
        data: { ...defaultSelectedBudget },
    })

    const user = ref<UserViewModel>(defaultUser); 

    const getCurrentUser = async () => {
    const url = '/api/User/Current';
    try {
        const response: ApiResponse<UserViewModel> = await get<UserViewModel>(url);
        if (response.StatusCode === 200) {
            const data = response.Data;
            user.value = data ? data : defaultUser;
            //console.log('user', user.value);
            searchGroup.value = user.value.GroupId;
            defaultBudgetAmount.RequestPerson = user.value.Name;
            defaultBudgetAmount.PaymentPerson = user.value.Name;
        }
        else {
            console.error(response.Data ?? response.Message);
        }
    }
    catch (error: any) {
        console.error(error.message);
    }
};
    const filteredGroups = computed(() => { //權限設定
        if (user.value.RolePermissionId === 3) {
            const group = groups.value.find(group => group.value === searchGroup.value);
            return group ? [{ text: group.text, value: group.value }] : [];
        }
        return groups.value;
    });

    const canAdd = computed(() => { //權限設定
        if (user.value.RolePermissionId === 3) {
            return false;
        }
        else {
            return true;
        }
    });

    const canEdit = (item: SelectedDetail) => { //權限設定
        //console.log('item: ',item);
        if (user.value.RolePermissionId === 1) {
            return true;
        }
        else if (user.value.RolePermissionId === 3) {
            return false;
        }
        else if (user.value.RolePermissionId === 2 && !item.Reconciled) {
            return item.RequestPerson === user.value.Name;  // 權限是B時資料的請購人與使用者相同時才能編輯資料,已對帳的只有A權限可以編輯
        }
        return false;
    };
    const canAllocate = computed(() => user.value.RolePermissionId === 1 ? true : false); //權限設定
    const previousPage = async () => {
        isSelectedItem.value = false;
        try {
            await fetchBudgetData(); // 重新取一次資料(不管有沒有更新或刪除)
            page.value = 1;
            detailPage.value = 1;
        }
        catch (error) {
            console.error(error);
        };
    };
  
    const fetchBudgetData = async () => {
        const url = '/api/BudgetAmount';
        //const data = { params: { Year: searchYear.value } }; 
        const data = { Year: searchYear.value, GroupId: searchGroup.value };  // 抓年度的值
        loading.value = true;
        try {
            const response: ApiResponse<BudgetAmountViewModel[]> = await get<BudgetAmountViewModel[]>(url, data);
            //console.log(response.Data);
            if (response.StatusCode === 200) {
                //console.log('data:', response.Data);
                //console.log(response.StatusCode);
                const dbData = response.Data?.map((item: BudgetAmountViewModel): Budget => ({
                    BudgetId: item.BudgetId,
                    BudgetName: item.Budget!.BudgetName,
                    GroupId: item.Budget!.GroupId,
                    GroupName: item.Budget!.Group!.GroupName,
                    Subject6: item.Budget!.Subject6,
                    Subject7: item.Budget!.Subject7,
                    Subject8: item.Budget!.Subject8,
                    AnnualBudgetAmount: item.Budget!.AnnualBudgetAmount,
                    FinalBudgetAmount: item.Budget!.FinalBudgetAmount,
                    Type: item.Type,
                    RequestAmount: item.RequestAmount,
                    PaymentAmount: item.PaymentAmount,
                    RequestDate: item.RequestDate || ""  // 確保不為 undefined
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

    const fetchSelectedDetail = async (BudgetId: number, Description?: string, RequestAmountStart?: number, RequestAmountEnd?: number, BudgetName?: string, GroupId?: number, Year?: number ) => {
        const url = '/api/BudgetAmount/SelectedDetail';
        //const data: any = { BudgetName, GroupId, Year };
        const data: any = { BudgetId, Year: searchYear.value };

        if (Description) data.Description = Description;
        if (RequestAmountStart) data.RequestAmountStart = RequestAmountStart;
        if (RequestAmountEnd) data.RequestAmountEnd = RequestAmountEnd;
        //if (RequestAmountStart !== undefined) data.RequestAmountStart = Number(RequestAmountStart);
        //if (RequestAmountEnd !== undefined) data.RequestAmountEnd = Number(RequestAmountEnd);
        //console.log(data);
        try {
            loading.value = true;
            const response: ApiResponse<BudgetAmountViewModel[]> = await get<BudgetAmountViewModel[]>(url, data);
            //console.log('data', response.Data);
            if (response.StatusCode === 200) {
                //filteredDetailItem.value = response.Data!;
                selectedDetailItem.value = response.Data?.map(item => ({
                    ...item,
                    ExTax: item.ExTax,
                    Reconciled: item.Reconciled,
                    FormattedRequestDate: formatDate(item.RequestDate || ""),
                    FormattedPaymentDate: formatDate(item.PaymentDate || "")
                })).sort((a, b) => new Date(b.RequestDate || "").getTime() - new Date(a.RequestDate || "").getTime()) ?? [];
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
        const groupedItems = groupBy(items.value, ['BudgetName', 'GroupName', 'Subject6', 'Subject7', 'Subject8', 'AnnualBudgetAmount', 'FinalBudgetAmount']);
        const sortedItems = Object.values(groupedItems).map(group => {
            const firstItem = group[0];
            const general = sumByCondition(group, 1, 'RequestAmount');
            const out = sumByCondition(group, 2, 'RequestAmount');
            const inValue = sumByCondition(group, 3, 'RequestAmount');
            const inActual = sumByCondition(group, 3, 'PaymentAmount');
            const subjectActual = inActual + sumByCondition(group, 1, 'PaymentAmount');

            // 將 Final 欄位從字串轉換成數字
            const FinalBudgetAmount = firstItem.FinalBudgetAmount || 0;
            const AnnualBudgetAmount = firstItem.AnnualBudgetAmount || 0;

            const useBudget = AnnualBudgetAmount + FinalBudgetAmount - out - general;
            const end = AnnualBudgetAmount + FinalBudgetAmount - general - out + inValue;
            const inBalance = inValue - inActual;

            return {
                ...firstItem,
                RequestDate: firstItem.RequestDate != "" ? firstItem.RequestDate : null, // 後端DateTime不匹配空字串
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
        //return sortedItems.sort((a, b) => new Date(b.RequestDate || "").getTime() - new Date(a.RequestDate || "").getTime());
        return sortedItems;
    });

    const handleBudgetClick = async (budget: SelectedBudgetModel) => {
        isSelectedItem.value = true;
        const data = [{ ...budget }]; // 將整個item複製
        //console.log('Budget clicked:', data);
        selectedItem.value = data.map((v) => {
            const newData: SelectedBudgetModel = {
                BudgetId: v.BudgetId,
                GroupId: v.GroupId,
                Subject6: v.Subject6,
                Subject7: v.Subject7,
                Subject8: v.Subject8,
                AnnualBudgetAmount: v.AnnualBudgetAmount || 0,
                FinalBudgetAmount: v.FinalBudgetAmount || 0,
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
        budget.BudgetId ? currentBudgetId.value = budget.BudgetId : ''; // 儲存 budgetValue
        defaultBudgetAmount.BudgetId = currentBudgetId.value;
        //console.log('currentId', currentBudgetId.value);
        //console.log(defaultBudgetAmount);
        await fetchSelectedDetail(budget.BudgetId);
    };

    const handleExcelClick = async (budget: BudgetAmountExcelViewModel) => {
        try {
            loading.value = true;
            budget.Year = searchYear.value;
            //console.log(budget);
            const url = '/api/BudgetAmount/ExportToExcel';
            const fileBlob = await postDataAndDownloadFile(url, budget);

            //const blobUrl = window.URL.createObjectURL(new Blob([blob]));
            const blobUrl = window.URL.createObjectURL(fileBlob);
            const link = document.createElement('a');
            link.href = blobUrl;
            link.setAttribute('download', `${searchYear.value}維護${budget.BudgetName}.xlsx`); // 指定下載的檔案名
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (error) {
            console.error('Error downloading the file', error);
        } finally {
            loading.value = false;
        }
    };


    const showDetailForm = ref<boolean>(false);
    const editingItem = ref<BudgetAmountViewModel>(defaultBudgetAmount);

    const editItem = async (item: SelectedDetail) => {
        showDetailForm.value = true;
        isEdit.value = true;
        // 解構賦值，排除 FormattedRequestDate 和 FormattedPaymentDate 欄位
        //console.log('console.log(item);', item);
        const { FormattedRequestDate, FormattedPaymentDate, ...filteredItem } = item;
        editingItem.value = { ...filteredItem };
        limitBudget.value = selectedItem.value[0].UseBudget ?? 0;
        //console.log(item);
        //console.log('Edit item:', editingItem.value);
    };

    const handleUpdate = async (updatedItem: BudgetAmountViewModel) => {
        // 編輯項目的處理邏輯
        //console.log('updatedItem', updatedItem);
        try {
            // 重查
            await fetchBudgetData();
            await fetchSelectedDetail(currentBudgetId.value);
            // 重新更新 selectedItem
            const budgetItem = computedItems.value.find(budget => budget.BudgetId === currentBudgetId.value);
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
        defaultBudgetAmount.CreatedYear = searchYear.value; //CreatedYear 賦值,使用者不能改
        defaultBudgetAmount.AmountYear = searchYear.value;
        editingItem.value = defaultBudgetAmount;
    };

    const handleCreate = async (newItem: any) => {
        // 處理新增邏輯
        await fetchBudgetData();
        await fetchSelectedDetail(currentBudgetId.value);
        // 重新更新 selectedItem
        const budgetItem = computedItems.value.find(budget => budget.BudgetId === currentBudgetId.value);
        if (budgetItem) {
            await handleBudgetClick(budgetItem);
        }
        //console.log('Created:', newItem);
    };

    const cancelEdit = () => {
        showDetailForm.value = false;
        editingItem.value = defaultBudgetAmount;
    };

    const deleteItem = async (data: BudgetAmountViewModel) => {
        // 刪除項目的處理邏輯
        const isConfirmed = confirm('你確定要刪除此項目嗎？');
        if (isConfirmed) {
            /*console.log('Delete item:', item);*/
            try {
                const url = '/api/BudgetAmount/SoftDelete';
                const response: ApiResponse<any> = await put<any>(url, data);
                if (response.StatusCode == 200) {
                    //console.log(response.Message);
                    await fetchBudgetData();
                    await fetchSelectedDetail(currentBudgetId.value);
                    // 重新更新 selectedItem
                    const budgetItem = computedItems.value.find(budget => budget.BudgetId === currentBudgetId.value);
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


    const handleSearch = async (payload: { Description: string, RequestAmountStart: number, RequestAmountEnd: number }) => {
        // 處理查詢邏輯
        //console.log('Description:', payload.Description, 'Number:', payload.RequestAmountStart, 'Number:', payload.RequestAmountEnd);
        await fetchSelectedDetail(currentBudgetId.value, payload.Description, payload.RequestAmountStart, payload.RequestAmountEnd);
    };

    const openAllocatePage = (item: SelectedBudgetModel) => {
        allocateForm.value.data = item;
        showAllocatePage.value = true;
    }

    const cancelAllocatePage = async () => {

        await fetchBudgetData();
        await fetchSelectedDetail(currentBudgetId.value);
        // 重新更新 selectedItem
        const budgetItem = computedItems.value.find(budget => budget.BudgetId === currentBudgetId.value);
        if (budgetItem) {
            await handleBudgetClick(budgetItem);
        }
        showAllocatePage.value = false;
    }

    const linkBudgetForm = ref({
        visible: false,
        data: { ...defaultBudgetAmount },
    });
    const linkBudgetData = async (item: BudgetAmountViewModel) => {
        //console.log('test');
        //console.log(item);
        const url = '/api/BudgetAmount/ByLinkBudgetData';
        let data;
        if (item.LinkedBudgetAmountId) {
            data = {
                LinkedBudgetAmountId: item.LinkedBudgetAmountId
            };
        }
        else {
            console.log('這個資料沒有關聯Id');
            return;
        }
       
        try {
            const response: ApiResponse<BudgetAmountViewModel> = await get<BudgetAmountViewModel>(url, data);
            //console.log(response.StatusCode);
            if (response.StatusCode == 200) {
                //console.log(response.Data);
                linkBudgetForm.value.data = response.Data ?? { ...defaultBudgetAmount };
                linkBudgetForm.value.visible = true;
            }
        }
        catch (error) {
            console.error(error);
        }
    }

    const cancelLinkBudgetForm = () => {
        linkBudgetForm.value.visible = false;
    }

    // pagination
    const page = ref(1);
    const itemsPerPage = 12;
    const pageCount = computed(() => Math.ceil(computedItems.value.length / itemsPerPage));
    const paginatedItems = computed(() => {
        const start = (page.value - 1) * itemsPerPage;
        const end = start + itemsPerPage;
        return computedItems.value.slice(start, end);
    });

    const detailPage = ref(1);
    const detailItemsPerPage = 10;
    const detailPageCount = computed(() => Math.ceil(selectedDetailItem.value.length / detailItemsPerPage));
    const detailPaginatedItems = computed(() => {
        const start = (detailPage.value - 1) * detailItemsPerPage;
        const end = start + detailItemsPerPage;
        return selectedDetailItem.value.slice(start, end);
    });



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