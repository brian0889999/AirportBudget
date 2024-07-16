import type { MoneyItem } from '@/types/apiInterface';
import axios from 'axios';

// 透過condition(篩選條件是item的Text欄位)找出那一欄位的field做總和
export const sumByCondition = (items: MoneyItem[], condition: string, field: keyof MoneyItem) => {
    return items
        .filter(item => item.Text === condition)
        .reduce((sum, item) => sum + (item[field] as number || 0), 0);
};

// 用於分組
export const groupBy = (array: MoneyItem[], keys: (keyof MoneyItem)[]) => {
    return array.reduce((result, currentValue) => {
        const key = keys.map(k => currentValue[k]).join('-');
        if (!result[key]) {
            result[key] = [];
        }
        result[key].push(currentValue);
        return result;
    }, {} as Record<string, MoneyItem[]>);
};

// 民國年轉換工具函數
export const formatDate = (dateString: any): string => {
    if (!dateString) return "";  // 處理空值
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return "";  // 處理無效日期
    const year = date.getFullYear() - 1911;
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    return `${year}/${month}/${day}`;
};

export const handleExcelClick = async (budget: string) => {
    try {
        const response = await axios.get(`/api/MoneyDb/ExportToExcel?budget=${budget}`, {
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


// 數字格式化
//export const formatNumber = (value: number): string  => {
//    if (!value) return '0';
//    return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
//}
//if (typeof String) {

//}
export const formatNumber = (number: any) => {

    if (number === undefined || number === null || isNaN(number)) {
        return '0';
    }

    if (number < 0) {
        return `(${Math.abs(number).toLocaleString()})`;
    }
    return number.toLocaleString();
};


//export const formatNumberWithColor = (number: number): string => {
//    if (number < 0) {
//        return `(<span style="color: red;">${Math.abs(number).toLocaleString()}</span>)`;
//    }
//    return number.toLocaleString();
//};
