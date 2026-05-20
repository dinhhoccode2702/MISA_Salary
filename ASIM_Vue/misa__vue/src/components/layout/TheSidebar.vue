<template>
  <aside class="the-sidebar" :class="{ collapsed: isCollapsed }">
    <div class="sidebar-overlay"></div>

    <div class="sidebar-inner">
      <div class="sidebar-content">
        <ul class="nav-list">
          <li
            v-for="item in menuItems"
            :key="item.id"
            class="nav-item flex align-center justify-between"
            :class="{ active: $route.path.startsWith(item.path) }"
            :data-tooltip="isCollapsed ? item.text : null"
            data-tooltip-position="right"
            @click="navigateTo(item.path)"
          >
            <div class="flex align-center">
              <span
                class="nav-icon ms-icon-base"
                :class="`ms-icon--${item.iconName}`"
              ></span>
              <span class="nav-text" v-if="!isCollapsed">{{ item.text }}</span>
            </div>

            <span
              v-if="item.hasSubMenu && !isCollapsed"
              class="nav-arrow ms-icon-base ms-icon--arrow-right"
            ></span>
          </li>
        </ul>
      </div>

      <div class="sidebar-footer">
        <div
          class="toggle-btn flex align-center"
          role="button"
          :aria-label="isCollapsed ? 'Mở rộng thanh điều hướng' : 'Thu gọn thanh điều hướng'"
          :data-tooltip="isCollapsed ? 'Mở rộng' : null"
          data-tooltip-position="right"
          @click="toggleCollapse"
        >
          <span
            class="icon-toggle ms-icon-base ms-icon--toggle-sidebar"
            :class="{ rotated: isCollapsed }"
          ></span>
          <span v-if="!isCollapsed" class="toggle-text">Thu gọn</span>
        </div>
      </div>
    </div>
  </aside>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const isCollapsed = ref(false);
const emit = defineEmits(['collapse-change']);

const menuItems = [
  { id: 'overview', text: 'Tổng quan', iconName: 'overview', path: '/overview' },
  { id: 'salary-composition', text: 'Thành phần lương', iconName: 'salary', path: '/salary-composition' },
  { id: 'payroll-template', text: 'Mẫu bảng lương', iconName: 'template', path: '/payroll-template' },
  { id: 'payroll-data', text: 'Dữ liệu tính lương', iconName: 'data', path: '/payroll-data', hasSubMenu: true },
  { id: 'calculate', text: 'Tính lương', iconName: 'calculate', path: '/calculate', hasSubMenu: true },
  { id: 'payment', text: 'Chi trả', iconName: 'payment', path: '/payment', hasSubMenu: true },
  { id: 'report', text: 'Báo cáo', iconName: 'report', path: '/report' },
  { id: 'settings', text: 'Thiết lập', iconName: 'settings-side', path: '/settings', hasSubMenu: true },
];

const navigateTo = (path) => {
  router.push(path);
};

const toggleCollapse = () => {
  isCollapsed.value = !isCollapsed.value;
  emit('collapse-change', isCollapsed.value);
};
</script>
<style scoped>
/* â”€â”€ Layout sidebar â”€â”€ */
.the-sidebar {
  width: var(--sidebar-width);
  height: calc(100vh - var(--header-height));
  background-color: var(--sidebar-bg);
  background-image: url('../../assets/img/slidebar-30-4.92017f7.png');
  background-repeat: no-repeat;
  background-position: bottom left;
  background-size: contain;
  color: var(--sidebar-text);
  position: fixed;
  top: var(--header-height);
  left: 0;
  transition: width 0.3s ease;
  z-index: 900;
  display: flex;
  flex-direction: column;
  flex-shrink: 0;
  user-select: none; /* TrÃ¡nh chá»n text khi click nhanh */
}

.the-sidebar.collapsed {
  width: var(--sidebar-collapsed-width);
}

/* Lá»›p overlay tá»‘i má» bÃªn trong sidebar */
.sidebar-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 1;
}

/* Inner container ná»•i trÃªn overlay */
.sidebar-inner {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  height: 100%;
}

.sidebar-content {
  flex: 1;
  padding-top: 12px;
}

/* â”€â”€ Nav item â”€â”€ */
.nav-item {
  height: 42px;
  padding: 0 12px;
  cursor: pointer;
  margin: 2px 12px;
  border-radius: 8px;
  transition: background-color 0.2s;
}

.nav-item:hover {
  background-color: var(--sidebar-hover);
}

/* Active state: ná»n xanh, icon + text tráº¯ng */
.nav-item.active {
  background-color: var(--color-primary);
}

.nav-item.active .nav-text {
  color: #fff;
}

/* â”€â”€ Icon há»‡ thá»‘ng webkit-mask â”€â”€
   MÃ u icon = background-color (khÃ´ng pháº£i background-image ná»¯a)
   - Default: xÃ¡m nháº¡t --sidebar-icon-color
   - Hover: sÃ¡ng hÆ¡n chÃºt
   - Active: tráº¯ng
*/
.nav-icon {
  margin-right: 8px;
  flex-shrink: 0;
  background-color: var(--sidebar-icon-color); /* XÃ¡m máº·c Ä‘á»‹nh */
  transition: background-color 0.2s;
}

.the-sidebar.collapsed .nav-item {
  justify-content: center;
  padding: 0;
}

.the-sidebar.collapsed .nav-icon {
  margin-right: 0;
}

.nav-item:hover .nav-icon {
  background-color: #cccccc; /* SÃ¡ng hÆ¡n khi hover */
}

.nav-item.active .nav-icon {
  background-color: #ffffff; /* Tráº¯ng khi active */
  /* KHÃ”NG cÃ²n dÃ¹ng: filter: brightness(0) invert(1); */
}

/* â”€â”€ Nav text â”€â”€ */
.nav-text {
  font-size: 14px;
  white-space: nowrap;
  color: var(--sidebar-icon-color); /* XÃ¡m máº·c Ä‘á»‹nh */
  transition: color 0.2s;
}

.nav-item:hover .nav-text {
  color: #cccccc;
}

.nav-item.active .nav-text {
  color: #ffffff;
}

/* â”€â”€ Arrow icon (mÅ©i tÃªn submenu) â”€â”€ */
.nav-arrow {
  background-color: var(--sidebar-icon-color);
  flex-shrink: 0;
  transition: background-color 0.2s;
}

.nav-item:hover .nav-arrow {
  background-color: #cccccc;
}

.nav-item.active .nav-arrow {
  background-color: #ffffff;
}

/* â”€â”€ Footer toggle button â”€â”€ */
.sidebar-footer {
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  padding: 12px;
  display: flex;
  justify-content: center;
}

.toggle-btn {
  cursor: pointer;
  opacity: 0.7;
  width: 100%;
  justify-content: center;
  gap: 8px;
  transition: opacity 0.2s;
}

.toggle-btn:hover {
  opacity: 1;
}

/* Icon toggle â€“ mÃ u xÃ¡m máº·c Ä‘á»‹nh */
.icon-toggle {
  background-color: var(--sidebar-icon-color);
  flex-shrink: 0;
  /* ThÃªm transition cho hiá»‡u á»©ng xoay */
  transition: transform 0.3s ease, background-color 0.2s;
}

/* Khi sidebar collapsed: xoay icon 180Â° */
.icon-toggle.rotated {
  transform: rotate(180deg);
}

.toggle-btn:hover .icon-toggle {
  background-color: #ffffff;
}

/* Text "Thu gá»n" */
.toggle-text {
  font-size: 13px;
  color: var(--sidebar-icon-color);
  white-space: nowrap;
}

.toggle-btn:hover .toggle-text {
  color: #ffffff;
}
</style>

