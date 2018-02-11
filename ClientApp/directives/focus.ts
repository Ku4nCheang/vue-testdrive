// Register a global custom directive called v-focus
export default {
    // When the bound element is inserted into the DOM...
    inserted: function (el: HTMLElement, binding: any, arg: string) {
      // Focus the element
      el.focus()
    }
}
