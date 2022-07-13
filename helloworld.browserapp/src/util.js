export function formatDate(jsonDate){
    let date = new Date(jsonDate);
    return `${date.getDay().toString().padStart(2, "0")}.${date.getMonth().toString().padStart(2, "0")}`;
}