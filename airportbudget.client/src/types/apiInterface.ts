export interface LoginViewModel {
    Account: string,
    Password:string
}

export interface UserViewModel {
    UserId: number,
    Name: string,
    Account: string,
    Password: string,
    RolePermissionId: number,
    GroupId: number,
    Status: boolean,
    System?: string,
    LastPasswordChangeDate?: Date,
    ErrCount: number,
    ErrDate: Date,
    Group?: {
        GroupId: number,
        GroupName: string
    },
    RolePermission?: {
        RolePermissionId: number,
        PermissionType: number
    },
}
export interface BudgetAmountViewModel {
    BudgetAmountId: number;
    Description: string;
    Type: number;
    RequestAmount: number;
    PaymentAmount: number;
    RequestDate: string;
    PaymentDate?: string;
    RequestPerson: string;
    PaymentPerson: string;
    ExTax: boolean;
    Reconciled: boolean;
    Remarks: string;
    Status: string;
    CreatedYear: number;
    AmountYear: number;
    BudgetId: number;
    //AmountSerialNumber: number;
    IsValid: boolean;
    Budget?: {
        BudgetId: number;
        BudgetName: string;
        Subject6: string;
        Subject7: string;
        Subject8: string;
        AnnualBudgetAmount: number;
        FinalBudgetAmount: number;
        CreatedYear: number;
        GroupId: number;
        Group?: {
            GroupId: number;
            GroupName: string;
        }
    }
}

export interface Budget {
    BudgetId: number,
    BudgetName: string,
    GroupId: number,
    GroupName: string,
    Subject6: string,
    Subject7: string,
    Subject8: string,
    AnnualBudgetAmount: number,
    FinalBudgetAmount: number,
    Type: number,
    RequestAmount: number,
    PaymentAmount: number,
    RequestDate: string,
}

export interface SelectedBudgetModel {
    BudgetId: number,
    BudgetName?: string,
    GroupId: number,
    GroupName?: string,
    Subject6: string,
    Subject7: string,
    Subject8?: string,
    AnnualBudgetAmount: number,
    FinalBudgetAmount: number,
    General: number,
    Out: number,
    UseBudget: number,
    In: number,
    InActual: number ,
    InBalance: number,
    SubjectActual: number,
    End: number,
    Type?: number,
    RequestAmount?: number,
    PaymentAmount?: number,
}

export interface SelectedDetail extends BudgetAmountViewModel {
    FormattedRequestDate?: string,
    FormattedPaymentDate?: string,
}

export interface GetIdDataViewModel {
    GroupId: number,
    Subject6: string,
    Subject7: string,
    Subject8: string
}

//export interface SelectedBudgetModel {
//    BudgetName?: string,
//    GroupName?: string,
//    Subject6?: string | null,
//    Subject7?: string | null,
//    Subject8?: string | null,
//    AnnualBudgetAmount?: number | null,
//    FinalBudgetAmount?: number | null,
//    General?: number | null,
//    Out?: number | null,
//    UseBudget?: number | null,
//    In?: number | null,
//    InActual?: number | null,
//    InBalance?: number | null,
//    SubjectActual?: number | null,
//    End?: number | null,
//    Type?: number,
//    RequestAmount?: number,
//    PaymentAmount?: number,
//}
//export interface SelectedBudgetModel {
//    Budget?: string,
//    Group?: string,
//    Subject6?: string | null,
//    Subject7?: string | null,
//    Subject8?: string | null,
//    BudgetYear?: number | null,
//    Final?: string | null,
//    General?: number | null,
//    Out?: number | null,
//    UseBudget?: number | null,
//    In?: number | null,
//    InActual?: number | null,
//    InBalance?: number | null,
//    SubjectActual?: number | null,
//    End?: number | null,
//    Text?: string,
//    PurchaseMoney?: number,
//    PayMoney?: number,
//}
export interface UserDataModel {
    No?: number,
    Name?: string,
    Account?: string,
    Password?: string,
    Auth?: string,
    Status1?: string,
    Status2?: string,
    Status3?: string,
}

export interface MoneyItem {
    Budget?: string,
    Group?: string,
    Subject6?: string,
    Subject7?: string,
    Subject8?: string,
    BudgetYear?: number,
    Final?: string,
    Text?: string,
    PurchaseMoney?: number,
    PayMoney?: number,
    Purchasedate?: string,
}
//export interface SelectedDetail {
//    ID1?: number,
//    Budget?: string,
//    Group?: string,
//    Subject6?: string,
//    Subject7?: string,
//    Subject8?: string,
//    BudgetYear?: number,
//    Final?: string,
//    Text?: string,
//    PurchaseMoney?: number,
//    PayMoney?: number,
//    Purchasedate?: string,
//    Remarks?: string,
//    PayDate?: string,
//    All?: string,
//    True?: string,
//    Year1?: string,
//}

export interface MoneyRawData {
    ID: number,
    Purchasedate?: string,
    Text?: string,
    Note?: string,
    PurchaseMoney?: number,
    PayDate?: string,
    PayMoney?: number,
    People?: string,
    Name?: string,
    Remarks?: string,
    People1?: string,
    ID1?: number,
    Status?: string,
    Group1?: string,
    Year?: number,
    Year1?: string,
    All?: string,
    True?: string,
    Money: {
        ID: number,
        Budget?: string,
        Group?: string,
        Subject6?: string,
        Subject7?: string,
        Subject8?: string,
        BudgetYear?: number,
        Final?: string,
        Year?: number,
    },
}

export interface SoftDeleteViewModel {
    ID: number,
    Purchasedate?: string,
    Text?: string,
    Note?: string,
    PurchaseMoney?: number,
    PayDate?: string | null,
    PayMoney?: number,
    People?: string,
    Name?: string,
    Remarks?: string,
    People1?: string,
    ID1?: number,
    Status?: string,
    Group1?: string,
    Year?: number,
    Year1?: string,
    All?: string,
    True?: string,
    Money: {
        ID: number,
        Budget?: string,
        Group?: string,
        Subject6?: string,
        Subject7?: string,
        Subject8?: string,
        BudgetYear?: number,
        Final?: string,
        Year?: number,
    },
}

export interface EditViewModel {
    ID: number,
    Purchasedate?: string,
    Text?: string,
    Note?: string,
    PurchaseMoney?: number,
    PayDate?: string,
    PayMoney?: number,
    People?: string,
    Name?: string,
    Remarks?: string,
    People1?: string,
    ID1?: number,
    Status?: string,
    Group1?: string,
    Year?: number,
    Year1?: string,
    All?: string,
    True?: string,
    Money: {
        ID: number,
        Budget?: string,
        Group?: string,
        Subject6?: string,
        Subject7?: string,
        Subject8?: string,
        BudgetYear?: number,
        Final?: string,
        Year?: number,
    },
}

//export interface Detail {
//    FormattedPurchasedate?: string,
//    FormattedPayDate?: string,
//    ID: number,
//    Purchasedate?: string,
//    Text?: string,
//    Note?: string,
//    PurchaseMoney?: number,
//    PayDate?: string,
//    PayMoney?: number,
//    People?: string,
//    Name?: string,
//    Remarks?: string,
//    People1?: string,
//    ID1?: number,
//    Status?: string,
//    Group1?: string,
//    Year?: number,
//    Year1?: string,
//    All?: string,
//    True?: string,
//    Money: {
//        ID: number,
//        Budget?: string,
//        Group?: string,
//        Subject6?: string,
//        Subject7?: string,
//        Subject8?: string,
//        BudgetYear?: number,
//        Final?: string,
//        Year?: number,
//    },
//}

export interface AllocateForm {
    BudgetAmountId: number,
    //AmountSerialNumber: number,
    Status: string,
    BudgetId: number,
    BudgetName: string,
    GroupId: number,
    InGroupId?: number,
    GroupName?: string,
    Subject6: string,
    Subject7: string,
    Subject8: string,
    RequestAmount: number,
    PaymentAmount: number,
    Subject6_1: string,
    Subject7_1: string,
    Subject8_1?: string,
    RequestPerson: string,
    CreatedYear: number,
    AmountYear: number,
    RequestDate: string,
    Description: string,
    PaymentPerson: string,
    //Budget: string,
    Remarks?: string,
    Type: number,
    ExTax: boolean,
    Reconciled: boolean,
}
export interface AllocateFormViewModel {
    ID: number,
    ID1: number,
    Status: string,
    Name: string,
    Group: string,
    Subject6: string,
    Subject7: string,
    Subject8: string,
    PurchaseMoney: number,
    PayMoney: number,
    Group1: string,
    Subject6_1: string,
    Subject7_1: string,
    Subject8_1?: string,
    People: string,
    Year: number,
    Year1: string,
    Purchasedate: string,
    Note: string,
    People1: string,
    //Budget: string,
    Remarks?: string,
    Text: string,
    All: string | null,
    True: string | null,
}

export interface AllocateFormViewModelOut {
    Group: string,
    Subject6: string,
    Subject7: string,
    Subject8?: string,
    PurchaseMoney: number,
    PayMoney: number,
    Group1: string,
    Subject6_1: string,
    Subject7_1: string,
    Subject8_1?: string,
    People: string,
    Year: number,
    PurchaseDate: string,
    Note: string,
    People1: string,
    Budget: string,
    Remarks?: string,
}