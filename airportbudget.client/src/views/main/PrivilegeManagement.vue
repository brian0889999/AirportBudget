<template>
    
    <v-container style="width:100%; display:flex;">
        <v-row>
            <v-col cols="12" sm="8" md="6">
                <v-btn v-if="!isEditing" @click="addItem" color="primary" class="mb-4">新增使用者</v-btn>
                <v-data-table v-if="!isEditing"
                              :headers="authheaders"
                              :items="paginatedItems"
                              item-key="Name"
                              items-per-page="12"
                              class="elevation-1"
                              style="width: 100%;"
                              hide-default-footer>
                    <!--這邊items-per-page要跟itemsPerPage一樣-->
                    <template v-slot:top>
                        <v-pagination v-model="page"
                                      :length="pageCount"
                                      @input="updatePage"
                                      class="mb-4"></v-pagination>
                    </template>
                    <template #item.RolePermissionId="{ item }">
                        {{ translateRolePermission(item.RolePermissionId) }}
                    </template>
                    <template #item.System="{ item }">
                        {{ translateSystem(item.System) }}
                    </template>
                    <template v-slot:item.Status="{ item }">
                        <v-chip :color="accountStatusColor(item.Status)">
                            {{ accountStatusStr(item.Status) }}
                        </v-chip>
                    </template>
                    <template v-slot:[`item.Edit`]="{ item }">
                        <v-btn @click="editItem(item)" icon class="small-btn">
                            <v-icon>mdi-pencil</v-icon>
                        </v-btn>
                    </template>
                </v-data-table>

            </v-col>
            <v-col cols="12" sm="8" md="6">
                <!--寫死的Table-->
                <AuthTable v-if="!isEditing" />
            </v-col>
        </v-row>

        <!-- 表格內容 -->
        <v-card v-if="isEditing" width="100%">
            <v-card-title>
                <v-row>
                    <v-col>
                        <h2>{{ privilegeManagementFormTitle }}</h2>
                    </v-col>
                </v-row>
            </v-card-title>
            <v-card-text>
                <v-form ref="privilegeManagementFormRef">
                    <v-text-field v-model="privilegeManagementForm.Name" label="姓名" :readonly="isEditMode"
                                  :bg-color="nameColumn"
                                  :rules="[rules.required]"></v-text-field>
                    <v-select v-model="privilegeManagementForm.RolePermissionId"
                              :items="RolePermissionItems"
                              item-title="text"
                              item-value="value"
                              label="權限"
                              :rules="[rules.required]"></v-select>
                    <v-select v-model="privilegeManagementForm.GroupId"
                              :items="ReverseGroupIdMapping" item-title="text" item-value="value"
                              label="組室"
                              :rules="[rules.required]"></v-select>
                    <v-select v-model="privilegeManagementForm.System"
                              :items="ReverseStatusMapping" item-title="text" item-value="value"
                              label="系統"></v-select>
                    <v-select v-model="privilegeManagementForm.Status"
                              :items="accountStatusOptions" item-title="text" item-value="value"
                              label="狀態*"></v-select>
                    <v-text-field v-model="privilegeManagementForm.Account" label="帳號"
                                  :rules="[rules.required]"></v-text-field>
                    <v-text-field v-model="privilegeManagementForm.Password" label="密碼"
                                  :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                                  :type="showPassword ? 'text' : 'password' "
                                  @click:append-inner="showPassword = !showPassword"
                                  :rules="[rules.passwordFormat]"></v-text-field>
                    <v-btn @click="saveItem" color="primary" class="mr-2" size="large">{{ saveBtn }}</v-btn>
                    <v-btn @click="cancelEdit" color="primary" size="large">取消</v-btn>
                </v-form>
            </v-card-text>
        </v-card>
    </v-container>

</template>


<script setup lang="ts">
import axios from 'axios';
import { ref, computed, onMounted } from 'vue';
import type { UserDataModel, UserViewModel } from '@/types/apiInterface';
import type { SelectedOption } from '@/types/vueInterface';
import type { VDataTable } from 'vuetify/components';
import { AuthMapping, ReverseGroupIdMapping, Status3Mapping, ReverseStatusMapping } from '@/utils/mappings'; // 對應狀態碼到中文
import { get, put, post, type ApiResponse } from '@/services/api';
import { RULES } from '@/constants/constants';
import  AuthTable  from '@/components/modules/AuthTable.vue';
type ReadonlyHeaders = VDataTable['$props']['headers'];

    const authheaders: ReadonlyHeaders = [
        { title: '姓名', align: 'start', sortable: false, key: 'Name', value: 'Name' },
        { title: '權限', key: 'RolePermissionId', value: 'RolePermissionId' },
        { title: '系統', key: 'System', value: 'System' },
        { title: '狀態', key: 'Status', value: 'Status' },
        { title: '編輯', key: 'Edit', value: 'Edit', sortable: false },
    ];

    const listForm = ref<UserViewModel[]>([]);
    const isEditing = ref<boolean>(false);
    const isEditMode = ref<boolean>(true); // 用來區分新增或編輯資料
    const toUTC = (date: Date) => {
        return new Date(date.getTime() - date.getTimezoneOffset() * 60000);
    };
    const defaultData: UserViewModel = {
        UserId: 0,
        Name: '',
        Account: '',
        Password: '',
        RolePermissionId: 1,
        GroupId: 1,
        Status: true,
        System: '',
        LastPasswordChangeDate: toUTC(new Date()),
        ErrCount: 0,
        ErrDate: toUTC(new Date(1990, 0, 1)),
        "Group": {
            "GroupId": 0,
            "GroupName": ""
        },
        "RolePermission": {
            "RolePermissionId": 0,
            "PermissionType": 0
        }
    };
    const privilegeManagementForm = ref<UserViewModel>(defaultData); // 表單的欄位資料
    //const status1Items = ref<Array<string>>(['A', 'B', 'C', 'D']);
    const RolePermissionItems :SelectedOption[] = [
        { text: 'A', value: 1 },
        { text: 'B', value: 2 },
        { text: 'C', value: 3 },
        //{ text: 'D', value: 4 },
    ];
    const accountStatusOptions: SelectedOption[] = [
        { text: '啟用', value: true },
        { text: '停用', value: false },
    ]
    const showPassword = ref<boolean>(false);
    const privilegeManagementFormRef = ref<HTMLFormElement | null>(null);
    // pagination
    const page = ref<number>(1);
    const itemsPerPage = ref<number>(12);
    const pageCount = computed(() => Math.ceil(listForm.value.length / itemsPerPage.value));
    const paginatedItems = computed(() => {
        const start = (page.value - 1) * itemsPerPage.value;
        const end = start + itemsPerPage.value;
        return listForm.value.slice(start, end);
    });
    const updatePage = (newPage: number) => {
        page.value = newPage;
    };



    const saveBtn = computed(() => isEditMode.value ? "保存" : "新增");
    const nameColumn = computed(() => isEditMode.value ? "grey-lighten-1" : "");
    const translateSystem = (System: string | undefined): string => {
        return System ? Status3Mapping[System] || System : '';
    };
    const privilegeManagementFormTitle = computed(() => isEditMode.value ? '編輯使用者' : '新增使用者');
    const translateRolePermission = (rolePermissionId: number): string  => {
        const roleMapping: { [key: number]: string } = {
            1: 'A',
            2: 'B',
            3: 'C',
            //4: 'D'
        };
        return roleMapping[rolePermissionId] || rolePermissionId.toString();
    }

    const accountStatusStr = (value: boolean) => {
        switch (value) {
            case true:
                return '啟用';
            case false:
                return '停用';
            default:
                return '';
        }
    };
    const accountStatusColor = (value: boolean) => {
        switch (value) {
            case true:
                return 'green';
            case false:
                return 'red';
            default:
                return '';
        }
    };
    //取得其他使用者
    const fetchUsers = async () => {
        try {
            const url = '/api/User'
            const response: ApiResponse<UserViewModel[]> = await get<UserViewModel[]>(url);
            /*console.log(response.Data);*/
       if (response && response.Data && response.Data) {
          listForm.value = response.Data;
          //console.log("listForm.value", response.Data);
        } else {
          console.error("Response data is null or undefined");
        }
        }
        catch (error) {
            console.error(error);
        }
    }

    const editItem = (item: UserViewModel) => {
        // 加入編輯邏輯
        console.log('Edit item:', item);
        isEditMode.value = true;
        isEditing.value = true;
        privilegeManagementForm.value = { ...item };
    };

    const addItem = () => {
        isEditMode.value = false;
        isEditing.value = true;
        privilegeManagementForm.value = defaultData;
    }

    const rules = RULES;
    const saveItem = async () => {
        console.log(privilegeManagementForm.value);
        const { valid } = await privilegeManagementFormRef.value?.validate();
        if (!valid) return;
        // 保存邏輯
        if (privilegeManagementForm.value) {
            //if (privilegeManagementForm.value.Status3 === "無") {
            //    privilegeManagementForm.value.Status3 = "";  // 將"無"轉換為空字串
            //};
            const url = '/api/User'
            const data: UserViewModel = privilegeManagementForm.value;
            try {
                let response: ApiResponse<any>;
                if (data.UserId) { // 如果是編輯用put,新增用post
                         response = await put<any>(url, data);
                        console.log(response.Data);
                }
                else
                {
                         response = await post<any>(url, data);
                        console.log(response.Data);
                };
                // 這裡可以加入成功處理的邏輯
                console.log("操作成功");
                await fetchUsers();
            } catch (error) {
                // 處理錯誤
                console.error(error);
                // 這裡可以加入錯誤處理的邏輯,例如提示用戶或記錄錯誤
            } finally {
                isEditing.value = false;
            }
        }
        /*console.log(privilegeManagementForm.value);*/
    };

    const cancelEdit = () => {
        isEditing.value = false;
        privilegeManagementForm.value = defaultData;
    };
    onMounted(fetchUsers);
</script>

<style scoped>
 .small-btn {
     transform: scale(0.8);
 }

    /*.custom-background .v-input__control {
        background-color: #e0e0e0;*/ /* 灰色背景 */
    /*}

    .custom-background input {
        pointer-events: none;*/ /* 禁止點擊 */
    /*}*/
</style>