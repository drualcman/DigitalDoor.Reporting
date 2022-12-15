
export function TabsSlide() {
let tabs = document.querySelectorAll('.tabs-item > li'),
  tabContent = document.querySelectorAll('.tab-pane');

    
    tabs.forEach((tab, index) => {
        tab.addEventListener('click', () => {
            tabContent.forEach((content) => {
                content.classList.remove('is-active');
            });
            tabs.forEach((tab) => {
                tab.classList.remove('is-active');
            });

            tabContent[index].classList.add('is-active');
            tabs[index].classList.add('is-active');
        })
    })
 
}