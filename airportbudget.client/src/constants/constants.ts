export const RULES = {
    required: (value: any) => !!value || "此欄位必填",
    passwordFormat: (value: any) => /^(?!.*[^\x21-\x7e])(?=.*[\W])(?=.*[a-zA-Z])(?=.*\d).{8,20}$/.test(value)
        || '請輸入 8 到 20 個字符的密碼，必須包含至少一個字母、一個數字和一個特殊字符。',
};
