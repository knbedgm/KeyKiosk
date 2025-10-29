export function set(name, value) {
    //debugger;
    window.cookieStore.set({name, value, sameSite: "strict"})
}

export function remove(name) {
    //debugger;
    window.cookieStore.delete({name})
}