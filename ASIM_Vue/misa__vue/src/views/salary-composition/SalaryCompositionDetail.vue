<template>
  <div class="salary-detail-container">
    <MsPageHeader 
          :title="pageTitle" 
          :showBack="true"
          @back="goBack"
        >
          <template #actions>
            <div class="flex align-center">
              <MsButton
                type="outline"
                bgColor = "#ffffff"
                class="m-r-12"
                @click="goBack"
              >
                {{ R.BTN_CANCEL }}
              </MsButton>
              <MsButton
                v-if="!isEdit"
                type="outline"
                bgColor = "#ffffff"
                class="m-r-12"
                @click="handleSaveAndAdd"
              >
                {{ R.BTN_SAVE_AND_ADD }}
              </MsButton>
              <MsButton
                type="primary"
                bgColor="#2ca01c"
                @click="handleSave"
              >
                {{ R.BTN_SAVE }}
              </MsButton>
            </div>
          </template>
        </MsPageHeader>
    <div class="main-view-container flex-column h-full">
      <div class="block-layout flex-1">
        <div class="detail-content">
          <div class="detail-form-container">
            <div class="form-section">
              <div class="form-row">
                <div class="form-label">{{ R.FIELD_NAME }} <span class="required">*</span></div>
                <div class="form-control">
                  <MsInput
                    ref="nameInputRef"
                    v-model="formData.SalaryCompositionName"
                    :placeholder="R.PLACEHOLDER_NAME"
                    :error="errors.SalaryCompositionName"
                    style="width: 100%;"
                  />
                </div>
              </div>

              <div class="form-row">
                <div class="form-label">{{ R.FIELD_CODE }} <span class="required">*</span></div>
                <div class="form-control">
                  <MsInput
                    ref="codeInputRef"
                    v-model="formData.SalaryCompositionCode"
                    :placeholder="R.PLACEHOLDER_CODE"
                    :error="errors.SalaryCompositionCode"
                    style="width: 300px;"
                  />
                </div>
              </div>

              <div class="form-row">
                <div class="form-label">{{ R.FIELD_APPLIED_UNIT }} <span class="required">*</span></div>
                <div class="form-control">
                  <MsSelect
                    ref="organizationInputRef"
                    v-model="formData.OrganizationId"
                    :options="organizationOptions"
                    :error="errors.OrganizationId"
                    style="width: 100%;"
                  />
                </div>
              </div>

              <div class="form-row">
                <div class="form-label">{{ R.FIELD_TYPE }} <span class="required">*</span></div>
                <div class="form-control">
                  <MsSelect
                    ref="typeInputRef"
                    v-model="formData.SalaryCompositionType"
                    :options="['Lương', 'Phụ cấp', 'Giảm trừ', 'Bảo hiểm - Công đoàn', 'Thuế TNCN', 'Chấm công', 'Thông tin nhân viên']"
                    :error="errors.SalaryCompositionType"
                    style="width: 300px;"
                  />
                </div>
              </div>

              <div class="form-row">
                <div class="form-label">{{ R.FIELD_NATURE }} <span class="required">*</span></div>
                <div class="form-control flex align-center">
                  <MsSelect
                    ref="natureInputRef"
                    v-model="formData.Nature"
                    :options="['Thu nhập', 'Khấu trừ', 'Khác']"
                    :error="errors.Nature"
                    style="width: 180px;"
                  />
                  <MsRadio
                    v-if="formData.Nature === 'Thu nhập'"
                    v-model="formData.TaxStatus"
                    name="tax_status"
                    :options="[
                      { label: 'Chịu thuế', value: 'Chịu thuế' },
                      { label: 'Miễn thuế toàn phần', value: 'Miễn thuế toàn phần' },
                      { label: 'Miễn thuế một phần', value: 'Miễn thuế một phần' },
                    ]"
                    class="m-l-24"
                  />
                </div>
              </div>

              <div class="form-row">
                <div class="form-label">{{ R.FIELD_QUOTA }}</div>
                <div class="form-control">
                  <MsFormula 
                    v-model="formData.Quota"
                    placeholder="Tự động gợi ý công thức và tham số khi gõ"
                    style="min-height: 80px;"
                  />
                  <div class="formula-helper-text m-t-4">Tự động gợi ý công thức và tham số khi gõ</div>
                  <div class="m-t-8">
                    <MsCheckbox
                      v-model="formData.AllowOverQuota"
                      :label="R.FIELD_ALLOW_OVER_QUOTA"
                      :title="R.TOOLTIP_ALLOW_OVER_QUOTA"
                    />
                  </div>
                </div>
              </div>

              <div class="form-row">
                <div class="form-label">{{ R.FIELD_VALUE_TYPE }}</div>
                <div class="form-control">
                  <MsSelect
                    v-model="formData.ValueType"
                    :options="['Tiền tệ', 'Số', 'Phần trăm', 'Chuỗi']"
                    style="width: 200px;"
                  />
                </div>
              </div>

              <div class="form-row">
                <div class="form-label">Giá trị</div>
                <div class="form-control">
                  <div class="p-b-8">
                    <MsRadio
                      v-model="formData.ValueSource"
                      name="value_source"
                      :options="[
                        { label: 'Tự động cộng tổng giá trị của các nhân viên', value: 'AutoSum' },
                      ]"
                    />
                    <div v-if="formData.ValueSource === 'AutoSum'" class="m-t-8 m-l-24">
                      <MsSelect
                        v-model="formData.AutoSumScope"
                        :options="['Trong cùng đơn vị công tác', 'Toàn công ty']"
                        style="width: 300px;"
                      />
                    </div>
                  </div>
                  <div class="p-v-8">
                    <MsRadio
                      v-model="formData.ValueSource"
                      name="value_source"
                      :options="[
                        { label: 'Tính theo công thức tự đặt', value: 'Formula' },
                      ]"
                    />
                    <div v-if="formData.ValueSource === 'Formula'" class="m-t-8">
                      <MsFormula 
                        v-model="formData.Value" 
                        placeholder="Tự động gợi ý công thức và tham số khi gõ"
                        :error="errors.Value"
                      />
                      <div class="formula-helper-text m-t-4">Tự động gợi ý công thức và tham số khi gõ</div>
                    </div>
                  </div>
                  <div class="p-v-8">
                    <MsRadio
                      v-model="formData.ValueSource"
                      name="value_source"
                      :options="[
                        { label: 'Nhập tay định mức/giá trị cố định', value: 'Constant' },
                      ]"
                    />
                    <div v-if="formData.ValueSource === 'Constant'" class="m-t-8">
                      <MsInput
                        v-model="formData.Value"
                        :format="formData.ValueType === 'Tiền tệ' ? 'currency' : 'number'"
                        placeholder="Nhập giá trị"
                        style="width: 200px;"
                      />
                    </div>
                  </div>
                </div>
              </div>

              <div class="form-row">
                <div class="form-label">{{ R.FIELD_DESCRIPTION }}</div>
                <div class="form-control">
                  <MsInput
                    ref="descriptionInputRef"
                    v-model="formData.Description"
                    type="text"
                    :error="errors.Description"
                    style="width: 100%;"
                  />
                </div>
              </div>

              <div class="form-row">
                <div class="form-label">Hiển thị trên phiếu lương</div>
                <div class="form-control">
                  <MsRadio
                    v-model="formData.DisplayOnPayslip"
                    name="display_payslip"
                    :options="[
                      { label: 'Có', value: 'Có' },
                      { label: 'Không', value: 'Không' },
                      { label: 'Chỉ hiển thị nếu giá trị khác 0', value: 'Khác 0' },
                    ]"
                  />
                </div>
              </div>

              <div class="form-row" style="margin-bottom: 0;">
                <div class="form-label">Nguồn tạo</div>
                <div class="form-control">
                  <span style="font-size: 13px; color: #666;">Tự thêm</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Confirm/Notify Modal (centered) -->
    <MsModal
      :show="dialog.show"
      :title="dialog.title"
      width="560px"
      @close="closeDialog"
    >
      <div class="dialog-content">
        <div class="dialog-message">{{ dialog.message }}</div>
      </div>
      <template #footer>
        <MsButton
          v-for="btn in dialog.buttons"
          :key="btn.key"
          :type="btn.type"
          :bgColor="btn.bgColor || ''"
          :textColor="btn.textColor || ''"
          class="m-l-8"
          @click="btn.onClick"
        >
          {{ btn.label }}
        </MsButton>
      </template>
    </MsModal>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';

import { useSalaryStore } from '@/stores/salaryStore';
import { SALARY_COMPOSITION as R } from '@/utils/resources';

import MsInput from '@/components/base/MsInput.vue';
import MsSelect from '@/components/base/MsSelect.vue';
import MsFormula from '@/components/base/MsFormula.vue';
import MsRadio from '@/components/base/MsRadio.vue';
import MsCheckbox from '@/components/base/MsCheckbox.vue';
import MsButton from '@/components/base/MsButton.vue';
import MsPageHeader from '@/components/layout/MsPageHeader.vue';
import MsModal from '@/components/common/MsModal.vue';
import { useToast } from '@/composables/useToast';
import organizationService from '@/services/organizationService';

const router = useRouter();
const route = useRoute();
const salaryStore = useSalaryStore();
const toast = useToast();

const nameInputRef = ref(null);
const codeInputRef = ref(null);
const typeInputRef = ref(null);
const natureInputRef = ref(null);
const organizationInputRef = ref(null);
const descriptionInputRef = ref(null);

const isClone = computed(() => route.query?.mode === 'clone' || !!route.query?.cloneFrom);
const isEdit = computed(() => !!route.params.id && !isClone.value);
const pageTitle = computed(() => {
  if (isClone.value) return R.PAGE_TITLE_CLONE;
  return isEdit.value ? R.PAGE_TITLE_EDIT : R.PAGE_TITLE_ADD;
});

const formData = ref({ ...salaryStore.currentItem });

const errors = ref({});
const MAX_TEXT_LENGTH = 255;

const organizationOptions = ref([{ value: null, label: 'Chọn đơn vị áp dụng' }]);

const dialog = ref({
  show: false,
  title: 'Thông báo',
  message: '',
  buttons: [],
});

const closeDialog = () => {
  dialog.value = { show: false, title: 'Thông báo', message: '', buttons: [] };
};

const showInfoDialog = (message, title = 'Thông báo') => {
  dialog.value = {
    show: true,
    title,
    message,
    buttons: [
      { key: 'close', label: 'Đóng', type: 'primary', onClick: closeDialog },
    ],
  };
};

const showConfirmDialog = ({ title = 'Thông báo', message, cancelLabel = 'Hủy bỏ', okLabel = 'Đồng ý', onOk }) => {
  dialog.value = {
    show: true,
    title,
    message,
    buttons: [
      { key: 'cancel', label: cancelLabel, type: 'outline', onClick: closeDialog },
      {
        key: 'ok',
        label: okLabel,
        type: 'primary',
        onClick: async () => {
          closeDialog();
          await onOk?.();
        },
      },
    ],
  };
};

const initialSnapshot = ref('');

const getComparableForm = (data) => {
  const src = data || {};
  return {
    SalaryCompositionName: src.SalaryCompositionName || '',
    SalaryCompositionCode: src.SalaryCompositionCode || '',
    OrganizationId: src.OrganizationId ?? null,
    SalaryCompositionType: src.SalaryCompositionType || '',
    Nature: src.Nature || '',
    TaxStatus: src.TaxStatus || '',
    Quota: src.Quota || '',
    AllowOverQuota: !!src.AllowOverQuota,
    ValueType: src.ValueType || '',
    ValueSource: src.ValueSource || '',
    AutoSumScope: src.AutoSumScope || '',
    Value: src.Value || '',
    Description: src.Description || '',
    DisplayOnPayslip: src.DisplayOnPayslip || '',
  };
};

const setInitialSnapshot = () => {
  initialSnapshot.value = JSON.stringify(getComparableForm(formData.value));
};

const isDirty = computed(() => {
  if (!initialSnapshot.value) return false;
  return JSON.stringify(getComparableForm(formData.value)) !== initialSnapshot.value;
});

const focusFirstError = (fieldRef) => {
  setTimeout(() => {
    fieldRef?.value?.focus?.();
  }, 0);
};

const normalizeTextFields = () => {
  formData.value.SalaryCompositionName = formData.value.SalaryCompositionName?.trim() || '';
  formData.value.SalaryCompositionCode = formData.value.SalaryCompositionCode?.trim() || '';
  formData.value.Description = formData.value.Description?.trim() || '';
};

const FIELD_ERROR_MAP = {
  SalaryCompositionName: 'SalaryCompositionName',
  salaryCompositionName: 'SalaryCompositionName',
  salary_composition_name: 'SalaryCompositionName',
  name: 'SalaryCompositionName',
  SalaryCompositionCode: 'SalaryCompositionCode',
  salaryCompositionCode: 'SalaryCompositionCode',
  salary_composition_code: 'SalaryCompositionCode',
  code: 'SalaryCompositionCode',
  OrganizationId: 'OrganizationId',
  organizationId: 'OrganizationId',
  organization_id: 'OrganizationId',
  SalaryCompositionComponentType: 'SalaryCompositionType',
  salaryCompositionComponentType: 'SalaryCompositionType',
  salary_composition_component_type: 'SalaryCompositionType',
  SalaryCompositionType: 'SalaryCompositionType',
  salaryCompositionType: 'SalaryCompositionType',
  SalaryCompositionNatureType: 'Nature',
  salaryCompositionNatureType: 'Nature',
  salary_composition_nature_type: 'Nature',
  Nature: 'Nature',
  nature: 'Nature',
  SalaryCompositionDescription: 'Description',
  salaryCompositionDescription: 'Description',
  salary_composition_description: 'Description',
  Description: 'Description',
  description: 'Description',
  SalaryCompositionValueFormula: 'Value',
  salaryCompositionValueFormula: 'Value',
  salary_composition_value_formula: 'Value',
  Value: 'Value',
  value: 'Value',
};

const normalizeBackendFieldKey = (key) => {
  if (!key) return '';
  const rawKey = String(key).split('.').pop().replace(/\[\d+\]/g, '');
  const camelKey = rawKey.charAt(0).toLowerCase() + rawKey.slice(1);
  return FIELD_ERROR_MAP[rawKey] || FIELD_ERROR_MAP[camelKey] || rawKey;
};

const getBackendErrorMessage = (value) => {
  if (Array.isArray(value)) return value.filter(Boolean).join('\n');
  if (value && typeof value === 'object') {
    return Object.values(value).flat().filter(Boolean).join('\n');
  }
  return value ? String(value) : '';
};

const getFieldRef = (field) => ({
  SalaryCompositionName: nameInputRef,
  SalaryCompositionCode: codeInputRef,
  OrganizationId: organizationInputRef,
  SalaryCompositionType: typeInputRef,
  Nature: natureInputRef,
  Description: descriptionInputRef,
})[field];

const applyBackendErrors = (backendErrors) => {
  if (!backendErrors || typeof backendErrors !== 'object') return false;

  const mappedErrors = {};
  let firstErrorRef = null;

  Object.entries(backendErrors).forEach(([key, value]) => {
    const field = normalizeBackendFieldKey(key);
    const message = getBackendErrorMessage(value);
    if (!field || !message) return;

    mappedErrors[field] = message;
    if (!firstErrorRef) firstErrorRef = getFieldRef(field);
  });

  if (!Object.keys(mappedErrors).length) return false;

  errors.value = { ...errors.value, ...mappedErrors };
  if (firstErrorRef) focusFirstError(firstErrorRef);
  return true;
};

const getSaveErrorMessage = () =>
  salaryStore.lastError?.userMsg ||
  salaryStore.lastError?.devMsg ||
  'Không thể lưu dữ liệu. Vui lòng thử lại.';

onMounted(async () => {
  const cloneFromId = route.query?.cloneFrom || (isClone.value ? route.params.id : null);

  if (isEdit.value) {
    await salaryStore.fetchSalaryCompositionById(route.params.id);
    formData.value = { ...salaryStore.currentItem };
  } else if (cloneFromId) {
    await salaryStore.fetchSalaryCompositionById(cloneFromId);
    const cloned = { ...salaryStore.currentItem };

    delete cloned.SalaryCompositionId;
    cloned.SalaryCompositionCode = '';
    cloned.SalaryCompositionName = '';
    cloned.Source = 'Tự thêm';

    formData.value = cloned;
  } else {
    salaryStore.setNewItem();
    formData.value = { ...salaryStore.currentItem };
  }

  setInitialSnapshot();
  
  setTimeout(() => {
    nameInputRef.value?.focus();
  }, 100);

  try {
    const orgRes = await organizationService.getAll();
    const orgService = orgRes?.data;
    const orgs = orgService?.Data ?? orgService?.data ?? orgService ?? orgRes ?? [];
    if (Array.isArray(orgs)) {
      organizationOptions.value = [
        { value: null, label: 'Chọn đơn vị áp dụng' },
        ...orgs.map((o) => {
          const id = o.organizationId ?? o.OrganizationId;
          const code = o.organizationCode ?? o.OrganizationCode;
          const name = o.organizationName ?? o.OrganizationName;
          return { value: id, label: `${code} - ${name}` };
        })
      ];
    }
  } catch (error) {
    console.error('Lỗi khi tải danh sách tổ chức:', error);
  }

});

watch(
  () => salaryStore.currentItem,
  (val) => {
    if (isClone.value) return;
    formData.value = { ...val };
    setInitialSnapshot();
  },
  { deep: true }
);

watch(
  () => formData.value.Nature,
  (newVal) => {
    if (newVal !== 'Thu nhập') {
      formData.value.TaxStatus = '';
    } else if (!formData.value.TaxStatus) {
      formData.value.TaxStatus = 'Chịu thuế';
    }
  }
);

const goBack = () => {
  if (isDirty.value) {
    dialog.value = {
      show: true,
      title: 'Thông báo',
      message: 'Dữ liệu đã thay đổi. Bạn có muốn lưu không?',
      buttons: [
        { key: 'cancel', label: 'Hủy', type: 'outline', onClick: closeDialog },
        {
          key: 'discard',
          label: 'Không lưu',
          type: 'outline',
          onClick: () => {
            closeDialog();
            router.push('/salary-composition');
          },
        },
        {
          key: 'save',
          label: 'Lưu',
          type: 'primary',
          onClick: async () => {
            closeDialog();
            await handleSave();
          },
        },
      ],
    };
    return;
  }

  showConfirmDialog({
    title: 'Thông báo',
    message: 'Bạn có muốn hủy bỏ và quay lại danh sách không?',
    cancelLabel: 'Hủy bỏ',
    okLabel: 'Đồng ý',
    onOk: () => router.push('/salary-composition'),
  });
};

const validate = () => {
  normalizeTextFields();
  const newErrors = {};
  let firstErrorRef = null;

  if (!formData.value.SalaryCompositionName) {
    newErrors.SalaryCompositionName = R.ERROR_NAME_REQUIRED;
    if (!firstErrorRef) firstErrorRef = nameInputRef;
  } else if (formData.value.SalaryCompositionName.length > MAX_TEXT_LENGTH) {
    newErrors.SalaryCompositionName = `Tên thành phần không được vượt quá ${MAX_TEXT_LENGTH} ký tự.`;
    if (!firstErrorRef) firstErrorRef = nameInputRef;
  }

  if (!formData.value.SalaryCompositionCode) {
    newErrors.SalaryCompositionCode = R.ERROR_CODE_REQUIRED;
    if (!firstErrorRef) firstErrorRef = codeInputRef;
  } else if (formData.value.SalaryCompositionCode.length > MAX_TEXT_LENGTH) {
    newErrors.SalaryCompositionCode = `Mã thành phần không được vượt quá ${MAX_TEXT_LENGTH} ký tự.`;
    if (!firstErrorRef) firstErrorRef = codeInputRef;
  }

  if (formData.value.Description && formData.value.Description.length > MAX_TEXT_LENGTH) {
    newErrors.Description = `Mô tả không được vượt quá ${MAX_TEXT_LENGTH} ký tự.`;
    if (!firstErrorRef) firstErrorRef = descriptionInputRef;
  }

  if (!formData.value.OrganizationId) {
    newErrors.OrganizationId = 'Đơn vị áp dụng không được để trống.';
    if (!firstErrorRef) firstErrorRef = organizationInputRef;
  }

  if (!formData.value.SalaryCompositionType) {
    newErrors.SalaryCompositionType = R.ERROR_TYPE_REQUIRED;
    if (!firstErrorRef) firstErrorRef = typeInputRef;
  }

  if (!formData.value.Nature) {
    newErrors.Nature = R.ERROR_NATURE_REQUIRED;
    if (!firstErrorRef) firstErrorRef = natureInputRef;
  }

  errors.value = newErrors;
  
  if (firstErrorRef) {
    focusFirstError(firstErrorRef);
  }

  return Object.keys(newErrors).length === 0;
};

const handleSave = async () => {
  if (!validate()) {
    return;
  }

  const ok = await toast.confirm({
    message: isEdit.value ? 'Bạn có chắc chắn muốn lưu thay đổi không?' : 'Bạn có chắc chắn muốn thêm mới và lưu không?',
    cancelLabel: 'Hủy',
    okLabel: 'Lưu',
  });
  if (!ok) return;

  const success = await salaryStore.saveSalaryComposition(
    formData.value,
    isEdit.value
  );

  if (success) {
    toast.show({
      message: isEdit.value ? 'Lưu thay đổi thành công.' : (isClone.value ? 'Nhân bản thành công.' : 'Thêm mới thành công.'),
      type: 'success',
    });
    router.push('/salary-composition');
  } else {
    const hasFieldErrors = applyBackendErrors(salaryStore.lastError?.errors);
    const msg = getSaveErrorMessage();
    toast.show({ message: msg, type: 'error' });
    if (!hasFieldErrors) showInfoDialog(msg);
  }
};

const handleSaveAndAdd = async () => {
  if (!validate()) {
    return;
  }

  const ok = await toast.confirm({
    message: 'Bạn có chắc chắn muốn lưu và thêm tiếp không?',
    cancelLabel: 'Hủy',
    okLabel: 'Lưu',
  });
  if (!ok) return;

  const success = await salaryStore.saveSalaryComposition(
    formData.value,
    false
  );

  if (success) {
    toast.show({ message: 'Lưu thành công. Bạn có thể nhập bản ghi mới.', type: 'success' });
    salaryStore.setNewItem();
    formData.value = { ...salaryStore.currentItem };
    errors.value = {};
    setInitialSnapshot();

    setTimeout(() => {
      nameInputRef.value?.focus();
    }, 100);
  } else {
    const hasFieldErrors = applyBackendErrors(salaryStore.lastError?.errors);
    const msg = getSaveErrorMessage();
    toast.show({ message: msg, type: 'error' });
    if (!hasFieldErrors) showInfoDialog(msg);
  }
};
</script>

<style scoped>
.salary-detail-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
  width: 100%;
  overflow: hidden;
  background-color: #efefef;
}

.main-view-container {
  width: 100%;
  padding: 0 16px 16px 24px;
  display: flex;
  flex-direction: column;
  flex: 1;
  min-height: 0;
  box-sizing: border-box;
}

.block-layout {
  background-color: #fff;
  border-radius: 4px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
  overflow: hidden;
  border: 1px solid #e0e0e0;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

.header-wrapper {
  background-color: transparent;
  z-index: 10;
  flex-shrink: 0;
  padding: 10px 0;
}

.detail-content {
  flex: 1;
  overflow-y: auto;
  min-height: 0;
  padding: 24px 32px;
  background-color: #fff;
}

.detail-form-container {
  width: 100%;
}

.form-row {
  display: flex;
  margin-bottom: 24px;
  align-items: flex-start;
}

.form-label {
  width: 200px;
  min-width: 200px;
  padding-top: 8px;
  font-size: 13px;
  font-weight: 700;
  color: #111;
}

.dialog-content {
  font-size: 13px;
  color: var(--text-primary);
}

.dialog-message {
  line-height: 1.5;
}

.form-label .required {
  color: #eb5757;
  margin-left: 4px;
}

.form-control {
  flex: 1;
  max-width: 600px;
}

.p-b-8  { padding-bottom: 8px; }
.p-v-8  { padding-top: 8px; padding-bottom: 8px; }
.m-t-4  { margin-top: 4px; }
.m-t-8  { margin-top: 8px; }
.m-l-24 { margin-left: 24px; }
.m-r-12 { margin-right: 12px; }
.flex   { display: flex; }
.flex-column { flex-direction: column; }
.h-full { height: 100%; }
.flex-1 { flex: 1; }
.align-center { align-items: center; }

/* Formula helper text style */
.formula-helper-text {
  font-size: 12px;
  font-style: italic;
  color: #666;
}

/* Custom scrollbar */
.detail-content::-webkit-scrollbar {
  width: 8px;
}
.detail-content::-webkit-scrollbar-thumb {
  background: #ccc;
  border-radius: 4px;
}
.detail-content::-webkit-scrollbar-track {
  background: #f1f1f1;
}
</style>
