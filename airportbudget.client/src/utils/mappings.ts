// 對應狀態碼到中文
export const Status3Mapping: { [key: string]: string } = {
    "": "無",
    "A": "土木",
    "B": "水電",
    "C": "建築",
    "D": "綜合",
    "E": "機械"
};

//export const Status3Mapping: { text: string, value: string }[] = [
//    { text:"", value: "無" },
//    { text:"A", value: "土木" },
//    { text:"B", value: "水電"},
//    { text:"C", value: "建築" },
//    { text:"D", value: "綜合"},
//    { text:"E", value: "機械" }
//];

//export const ReverseStatusMapping: { [key: string]: string } = { // 轉換資料後存進資料庫
//    "無": "",
//    "土木": "A",
//    "水電": "B",
//    "建築": "C",
//    "綜合": "D",
//    "機械": "E"
//};
export const ReverseStatusMapping: { text: string, value: string }[] = [ // 轉換資料後存進資料庫
    { text: "無", value: "" },
    { text: "土木", value: "A"},
    { text: "水電", value: "B"},
    { text: "建築", value: "C"},
    { text: "綜合", value: "D"},
    { text: "機械", value: "E"}
];
export const AuthMapping: { [key: string]: string } = {
    "A": "工務組",
    "B": "業務組",
    "C": "人事室",
    "D": "中控室",
    "E": "北竿站",
    "F": "企劃組",
    "G": "南竿站",
    "H": "政風室",
    "I": "航務組",
    "J": "總務組"
}

//export const ReverseAuthMapping: { [key: string]: string } = { // 轉換資料後存進資料庫
//    "工務組": "A",
//    "業務組": "B",
//    "人事室": "C",
//    "中控室": "D",
//    "北竿站": "E",
//    "企劃組": "F",
//    "南竿站": "G",
//    "政風室": "H",
//    "航務組": "I",
//    "總務組": "J"
//}

export const ReverseGroupIdMapping: { text: string, value: number }[] = [
    { text: '工務組', value: 1 },
    { text: '業務組', value: 2 },
    { text: '人事室', value: 3 },
    { text: '中控室', value: 4 },
    { text: '北竿站', value: 5 },
    { text: '企劃組', value: 6 },
    { text: '南竿站', value: 7 },
    { text: '政風室', value: 8 },
    { text: '航務組', value: 9 },
    { text: '總務組', value: 10 },
    { text: '企劃行政組', value: 11 },
    { text: '營運安全組', value: 12 },
];


//export const TypeMapping: { text: string, value: number }[] = [
//    { text: '一般', value: 1 },
//    { text: '勻入', value: 2 },
//    { text: '勻出', value: 3 },
//];

export const TypeMapping: { [key: number] : string } = {
    1 : "一般" ,
    2 : "勻出",
    3 : "勻入",
};


export const groupMapping: Record<number, string> = {
    1: '工務組',
    2: '業務組',
    3: '人事室',
    4: '中控室',
    5: '北竿站',
    6: '企劃組',
    7: '南竿站',
    8: '政風室',
    9: '航務組',
    10: '總務組',
    11: '企劃行政組',
    12: '營運安全組'
};

export const systemOptionsMapping: Record<number, string> = {
    101: '土木',
    102: '水電',
    103: '建築',
    104: '綜合',
    105: '機械',
};
