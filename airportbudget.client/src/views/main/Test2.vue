<!--<template>
    <v-container>
        <v-row>
            <v-col cols="12" sm="6" md="3">
                <v-text-field v-model="titleKey" variant="outlined" label="標題關鍵字" clearable></v-text-field>
            </v-col>
            <v-col cols="12" sm="6" md="3">
                <v-text-field v-model="minRating" variant="outlined" label="最低評分" type="number" clearable></v-text-field>
            </v-col>
            <v-col cols="12" sm="6" md="3">
                <v-text-field v-model="minRatingCount" variant="outlined" label="最低評分次數" type="number" clearable></v-text-field>
            </v-col>
            <v-col cols="12" sm="6" md="3">
                <v-text-field v-model="city" variant="outlined" label="城市名稱" clearable></v-text-field>
            </v-col>
            <v-col cols="12" sm="6" md="3">
                <v-select v-model="category"
                          :items="categories"
                          item-title="text"
                          item-value="value"
                          label="類別"></v-select>
            </v-col>
            <v-col cols="12" sm="6" md="3">
                <v-select v-model="compareData"
                          :items="compareDatas"
                          item-title="text"
                          item-value="value"
                          label="相似"></v-select>
            </v-col>
            <v-col cols="12" sm="6" md="3">
                <v-btn :disabled="loading"
                       :loading="loading"
                       class="ma-2 pa-2"
                       size="x-large"
                       color="primary"
                       @click="searchItem">查詢</v-btn>
                <v-btn :disabled="loading"
                       :loading="loading"
                       class="ma-2 pa-2"
                       size="x-large"
                       color="primary"
                       @click="updateItem">更新數據</v-btn>
            </v-col>
        </v-row>

        <v-card>
            <v-tabs v-model="tab"
                    bg-color="primary">
                <v-tab value="table">表單</v-tab>
                <v-tab value="chart">圖表</v-tab>
            </v-tabs>

            <v-card-text>
                <v-window v-model="tab">
                    <keep-alive>
                        <v-window-item value="table">
                            <v-data-table :headers="headers"
                                          :items="desserts"
                                          no-data-text="暫無資料"
                                          loading-text="讀取中請稍後..."
                                          items-per-page-text="每頁筆數"
                                          :loading="loading">
                                <template v-slot:item.ttt="{ item }">
                                    {{ item.ReviewCount * item.ParticipantCount}}
                                </template>
                                <template v-slot:item.ParticipantCount="{ item }">
                                    {{ $formatNumberWithCommas(item.ParticipantCount) }}
                                </template>
                                <template v-slot:item.SalePrice="{ item }">
                                    {{ $formatNumberWithCommas(item.SalePrice) }}
                                </template>
                                <template v-slot:item.SaleStatus="{ item }">
                                    <template v-if="item.SaleStatus !== null">
                                        <v-icon :color="item.SaleStatus ? 'green' : 'red'">
                                            {{ item.SaleStatus ? 'mdi-check-circle' : 'mdi-close-circle' }}
                                        </v-icon>
                                    </template>
                                </template>
                                <template v-slot:item.WebLink="{ item }">
                                    <v-icon @click="openWebLink(item.WebLink)">mdi-open-in-new</v-icon>
                                </template>
                                <template v-slot:item.Category="{ item }">
                                    <img :src="getImagePath(item.Category)" style="width: 30px; height: 30px;" />
                                </template>
                                <template v-slot:item.TripDotComs="{ item }">
                                    <template v-if="item.TripDotComs !== null && item.TripDotComs.length > 0">
                                        <v-icon @click="showDetails(item)">mdi-magnify</v-icon>
                                    </template>
                                </template>
                            </v-data-table>
                        </v-window-item>
                    </keep-alive>
                    <keep-alive>
                        <v-window-item value="chart">
                            <canvas ref="barChartCanvas"></canvas>
                        </v-window-item>
                    </keep-alive>
                </v-window>
            </v-card-text>
        </v-card>
    </v-container>-->
    <!--<DetailsDialog v-model="detailsDialog.visible" :title="detailsDialog.title" :desserts="detailsDialog.desserts" />
    <MessageDialog v-model="messageDialog.visible" :title="messageDialog.title" :message="messageDialog.message" :iconConfig="messageDialog.iconConfig" />
    <UpdateDialog v-model="updateDialog.visible" :datas="updateDialog.datas" />-->
<!--</template>

<script setup lang="ts">
    import { ref, onMounted, watch, nextTick } from 'vue';
    import type { VDataTable } from 'vuetify/components';
    import { useRoute } from 'vue-router';
    /*import Chart from 'chart.js/auto';*/
    import { get, post, type ApiResponse } from '@/services/api';
    //import type { ProductQueryViewModel, ProductViewModel } from '@/types/apiInterface';
    //import type { SelectedOption, DetailsDialogConfig, MessageDialogConfig, UpdateDialogConfig, ProgressLinear } from '@/types/vueInterface';
    //import DetailsDialog from '@/components/DetailsDialog.vue';
    //import MessageDialog from '@/components/MessageDialog.vue';
    //import UpdateDialog from '@/components/UpdateDialog.vue';

    //import imgKlookLogo from '@/assets/images/klook_logo.jpg';
    //import imgKKDayLogo from '@/assets/images/kkday_logo.jpg';
    //import imgLiontravelLogo from '@/assets/images/liontravel_logo.jpg';
    //import imgTripDotComLogo from '@/assets/images/tripdotcom_logo.jpg';

    type ReadonlyHeaders = VDataTable['$props']['headers'];

    //const detailsDialog = ref<DetailsDialogConfig>({
    //    visible: false,
    //    title: '',
    //    desserts: [],
    //});

    //const messageDialog = ref<MessageDialogConfig>({
    //    visible: false,
    //    title: '',
    //    message: '',
    //    iconConfig: null,
    //});

    //const updateDialog = ref<UpdateDialogConfig>({
    //    visible: false,
    //    datas: []
    //});

    // 獲取當前網址的查詢參數對象
    //const route = useRoute()
    //const tokenValue = route.query.token;

    //if (tokenValue != 'ArtiChiang') {
    //    window.location.href = '/';
    //}

    const loading = ref<boolean>(false);
   /* const desserts = ref<ProductViewModel[]>([]);*/
    const tab = ref<string | null>(null);

    const titleKey = ref<string | null>(null);
    const minRating = ref<number | null>(null);
    const minRatingCount = ref<number | null>(null);
    const city = ref<string | null>(null);
    const category = ref<number>(0);
    const compareData = ref<boolean | null>(null);

    const barChartCanvas = ref<HTMLCanvasElement | null>(null);
   /* let barChart: Chart | null = null;*/
    let chartInitialized = false;

    //const categories = ref<SelectedOption[]>([]);

    //const compareDatas: SelectedOption[] = [
    //    { text: '全部', value: null },
    //    { text: '是', value: true },
    //    { text: '否', value: false },
    //];

    const headers: ReadonlyHeaders = [
        { title: '標題', align: 'start', sortable: true, key: 'Title', width: '39%' },
        { title: '城市名稱', align: 'start', sortable: true, key: 'City', width: '15%' },
        { title: '評分', align: 'end', sortable: true, key: 'Rating', width: '5%' },
        { title: '評論數', align: 'end', sortable: true, key: 'ReviewCount', width: '7%' },
        { title: '參加人數', align: 'end', sortable: true, key: 'ParticipantCount', width: '7%' },
        { title: '銷售價格', align: 'end', sortable: true, key: 'SalePrice', width: '7%' },
        { title: '狀態', align: 'end', sortable: true, key: 'SaleStatus', width: '5%' },
        { title: '連結', align: 'center', sortable: false, key: 'WebLink', width: '5%' },
        { title: '類型', align: 'center', sortable: false, key: 'Category', width: '5%' },
        { title: '相似', align: 'center', sortable: false, key: 'TripDotComs', width: '5%' },
        { title: '總收入', align: 'center', sortable: false, key: 'ttt', width: '5%' },
    ];

    onMounted(async () => {
        const url = '/api/EnumDescription';
        //try {
        //    const response: ApiResponse<any> = await get<any>(`${url}/CategoryType`);
        //    if (response.StatusCode === 200) {
        //        const data = response.Data;
        //        categories.value = data || [];
        //    } else {
        //        openErrorDialog(response.Message);
        //    }
        //} catch (error: any) {
        //    openErrorDialog(error.message);
        //}
    });

    watch(tab, (newTab, oldTab) => {
        //if (newTab === 'chart') {
        //    nextTick(() => {
        //        if (!chartInitialized) {
        //            createChartInstance();
        //            chartInitialized = true;
        //        }
        //        updateChartData();
        //    });
        //}
    });

    const searchItem = async () => {
        loading.value = true;

        const url = '/api/Product';
        //const queryParams: ProductQueryViewModel = {
        //    TitleKey: titleKey.value,
        //    MinRating: minRating.value,
        //    MinRatingCount: minRatingCount.value,
        //    City: city.value,
        //    Category: category.value,
        //    CompareData: compareData.value,
        //};

        //try {
        //    const response: ApiResponse<ProductViewModel[]> = await get<ProductViewModel[]>(url, queryParams);
        //    if (response.StatusCode === 200) {
        //        const data = response.Data;
        //        desserts.value = data || [];
        //        updateChartData();
        //    } else {
        //        openErrorDialog(response.Message);
        //    }
        //} catch (error: any) {
        //    openErrorDialog(error.message);
        //} finally {
        //    loading.value = false;
        //}
    };

    //const updateItem = async () => {
    //    loading.value = true;
    //    showUpdate();

    //    const urls = ['/api/Klook', '/api/KKDay', '/api/Liontravel', '/api/TripDotCom'];
    //    const urlLabelMap: Record<string, string> = {
    //        '/api/Klook': 'Klook',
    //        '/api/KKDay': 'KKDay',
    //        '/api/Liontravel': '雄獅旅遊',
    //        '/api/TripDotCom': 'Trip.Com'
    //    };

        // 顯示所有項目更新中的訊息
        //const updatePromises = urls.map(async (url) => {
        //    const targetLabel = urlLabelMap[url];
        //    try {
        //        const response: ApiResponse<any> = await post<any>(url);
        //        if (response.StatusCode === 200) {
        //            updateDialogItem(targetLabel, false, true);
        //        } else {
        //            updateDialogItem(targetLabel, false, false);
        //        }
        //    } catch (error: any) {
        //        updateDialogItem(targetLabel, false, false);
        //    }
        //});

        // 等待所有 API 請求完成
    //    try {
    //        await Promise.all(updatePromises);
    //        openSuccessDialog('所有項目更新完成！');
    //        await searchItem();
    //    } catch (error: any) {
    //        openErrorDialog(error.message);
    //    } finally {
    //        //updateDialog.value.visible = false;
    //        loading.value = false;
    //    }
    //};

    //const createChartInstance = () => {
    //    if (barChartCanvas.value) {
    //        const ctx = barChartCanvas.value.getContext('2d');

    //        if (ctx) {
    //            barChart = new Chart(ctx, {
    //                type: 'bar',
    //                data: {
    //                    labels: [],
    //                    datasets: [{
    //                        label: '各縣市產品量',
    //                        data: [],
    //                        backgroundColor: 'rgba(54, 162, 235, 0.2)',
    //                        borderColor: 'rgba(54, 162, 235, 1)',
    //                        borderWidth: 1
    //                    }]
    //                },
    //                options: {
    //                    scales: {
    //                        x: {
    //                            ticks: {
    //                                color: 'white'
    //                            }
    //                        },
    //                        y: {
    //                            ticks: {
    //                                color: 'white'
    //                            },
    //                            beginAtZero: true,
    //                        }
    //                    },
    //                    plugins: {
    //                        legend: {
    //                            labels: {
    //                                font: {
    //                                    size: 20
    //                                },
    //                                color: 'white'
    //                            }
    //                        }
    //                    }
    //                }
    //            });
    //        }
    //    }
    //}

    //const updateChartData = () => {
    //    if (barChart) {
    //        const uniqueCities = Array.from(
    //            new Set(
    //                desserts.value.flatMap(item => {
    //                    let cityString = item.City.trim(); // 移除城市名稱前後的空白
    //                    if (cityString === '') {
    //                        return ['無']; // 如果城市名稱為空白，則將其設置為 '無'
    //                    }
    //                    cityString = cityString.replace('所有城市', '***');
    //                    cityString = cityString.replace(/縣|市/g, '');
    //                    cityString = cityString.replace('***', '所有城市');
    //                    return cityString.split(',');
    //                })
    //            )
    //        );
    //        console.log(uniqueCities);

    //        let cityCounts = new Map();

    //        uniqueCities.forEach(city => {
    //            let count;
    //            if (city === '無') {
    //                // 如果城市名稱為空白，將其設置為 '無'
    //                count = desserts.value.filter(item => item.City.trim() === '').length;
    //            } else {
    //                // 否則，計算非空白城市的數量
    //                count = desserts.value.filter(item => item.City.includes(city)).length;
    //            }
    //            cityCounts.set(city, count);
    //        });

    //        // 排序城市數量
    //        const sortedCities = Array.from(cityCounts)
    //            .sort((a, b) => b[1] - a[1]);

    //        const labels = sortedCities.map(city => city[0]);
    //        const data = sortedCities.map(city => city[1]);

    //        barChart.data.labels = labels;
    //        barChart.data.datasets[0].data = data;

    //        barChart.update(); // 更新图表
    //    }
    //};

    //const openSuccessDialog = (message: string) => {
    //    messageDialog.value.visible = true;
    //    messageDialog.value.title = '成功';
    //    messageDialog.value.message = `${message}`;
    //    messageDialog.value.iconConfig = {
    //        material: 'mdi-check-circle',
    //        color: 'success',
    //    };
    //}

    //const openErrorDialog = (message: string) => {
    //    messageDialog.value.visible = true;
    //    messageDialog.value.title = '失敗';
    //    messageDialog.value.message = `${message}`;
    //    messageDialog.value.iconConfig = {
    //        material: 'mdi-alert-circle',
    //        color: 'error',
    //    };
    //};

    const openWebLink = (webLink: string) => {
        window.open(webLink, '_blank');
    };

    //const getImagePath = (categoryValue: string) => {
    //    if (categoryValue == '1') {
    //        return imgKlookLogo;
    //    } else if (categoryValue == '2') {
    //        return imgKKDayLogo;
    //    } else if (categoryValue == '3') {
    //        return imgLiontravelLogo;
    //    } else if (categoryValue == '4') {
    //        return imgTripDotComLogo;
    //    } else {
    //        return '';
    //    }
    //};

    //const showDetails = (values: ProductViewModel) => {
    //    detailsDialog.value.visible = true;
    //    detailsDialog.value.title = values.Title;
    //    detailsDialog.value.desserts = values.TripDotComs ?? [];
    //};

    //const showUpdate = () => {
    //    updateDialog.value.visible = true;
    //    updateDialog.value.datas = [];

    //    const labels = ['Klook', 'KKDay', '雄獅旅遊', 'Trip.Com'];
    //    labels.forEach((label) => {
    //        const data: ProgressLinear = {
    //            label: label,
    //            loading: true,
    //            result: false,
    //        };
    //        updateDialog.value.datas.push(data);
    //    });
    //};

    //const updateDialogItem = (label: string, loading: boolean, result: boolean) => {
    //    const dataToUpdate = updateDialog.value.datas.find(data => data.label === label);
    //    if (dataToUpdate) {
    //        dataToUpdate.loading = loading;
    //        dataToUpdate.result = result;
    //    }
    //}
</script>
<style scoped>
    .v-data-table__tr:hover {
        background-color: #232F34;
    }
</style>-->