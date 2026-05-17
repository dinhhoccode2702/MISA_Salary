<template>
  <div
    class="ms-formula"
    :class="{ 'has-error': displayError, 'is-valid': validationStatus === 'success' }"
  >
    <div class="formula-header flex align-center p-b-8">
      <label v-if="label" class="ms-label">{{ label }}</label>
    </div>

    <div class="editor-outer border-radius-4">
      <div class="editor-container">
        <prism-editor
          class="my-editor"
          v-model="formulaValue"
          :highlight="highlightFormula"
          line-numbers
          :placeholder="placeholder"
        />
      </div>

      <div class="formula-footer flex justify-between align-center p-x-12 p-y-8">
        <span class="hint">Sử dụng toán tử +, -, *, /, hàm Excel và mã thành phần lương.</span>
        <button
          type="button"
          class="btn-check-logic flex align-center"
          @click="checkLogic"
        >
          <span class="ms-icon-base ms-icon--check-green m-r-4"></span>
          <span>Kiểm tra logic</span>
        </button>
      </div>
    </div>

    <span v-if="displayError" class="error-msg">{{ displayError }}</span>
    <span
      v-else-if="validationMessage"
      class="formula-status"
      :class="`is-${validationStatus}`"
    >
      {{ validationMessage }}
    </span>
  </div>
</template>

<script setup>
import { computed, ref, watch } from 'vue';
import { PrismEditor } from 'vue-prism-editor';
import { useToast } from '@/composables/useToast';
import 'vue-prism-editor/dist/prismeditor.min.css';
import 'prismjs/themes/prism.css';

const props = defineProps({
  modelValue: String,
  label: String,
  placeholder: { type: String, default: 'Ví dụ: = LUONG_CO_BAN * HE_SO_LUONG' },
  error: String,
});

const emit = defineEmits(['update:modelValue', 'validated']);
const toast = useToast();

const localError = ref('');
const validationMessage = ref('');
const validationStatus = ref('');

const EXCEL_FUNCTIONS = new Set([
  'ABS',
  'AND',
  'AVERAGE',
  'COUNT',
  'IF',
  'IFS',
  'INT',
  'MAX',
  'MIN',
  'MOD',
  'NOT',
  'OR',
  'ROUND',
  'ROUNDDOWN',
  'ROUNDUP',
  'SUM',
  'SUMIF',
  'SUMIFS',
]);

const formulaValue = computed({
  get: () => props.modelValue || '',
  set: (val) => {
    localError.value = '';
    validationMessage.value = '';
    validationStatus.value = '';
    emit('update:modelValue', val);
  },
});

const displayError = computed(() => props.error || localError.value);

watch(
  () => props.error,
  (error) => {
    if (error) {
      validationMessage.value = '';
      validationStatus.value = '';
    }
  }
);

const escapeHtml = (value) =>
  String(value)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;');

const highlightFormula = (code = '') => {
  const tokenPattern = /(\[[^\]\n]*\]|"[^"\n]*"|'[^'\n]*'|\b[A-Za-z_][A-Za-z0-9_]*\b|\d+(?:[.,]\d+)?|[()+\-*\/^=,;<>])/g;
  let highlighted = '';
  let lastIndex = 0;

  code.replace(tokenPattern, (token, ...args) => {
    const offset = args[args.length - 2];
    highlighted += escapeHtml(code.slice(lastIndex, offset));
    lastIndex = offset + token.length;
    const safeToken = escapeHtml(token);

    if (/^\[[^\]\n]*\]$/.test(token)) {
      highlighted += `<span class="formula-token component">${safeToken}</span>`;
      return token;
    }

    if (/^["']/.test(token)) {
      highlighted += `<span class="formula-token string">${safeToken}</span>`;
      return token;
    }

    if (/^\d/.test(token)) {
      highlighted += `<span class="formula-token number">${safeToken}</span>`;
      return token;
    }

    if (/^[()+\-*\/^=,;<>]$/.test(token)) {
      highlighted += `<span class="formula-token operator">${safeToken}</span>`;
      return token;
    }

    const upperToken = token.toUpperCase();
    if (EXCEL_FUNCTIONS.has(upperToken)) {
      highlighted += `<span class="formula-token function">${safeToken}</span>`;
      return token;
    }

    if (/^[A-Z_][A-Z0-9_]*$/.test(token)) {
      highlighted += `<span class="formula-token component">${safeToken}</span>`;
      return token;
    }

    highlighted += `<span class="formula-token identifier">${safeToken}</span>`;
    return token;
  });

  return highlighted + escapeHtml(code.slice(lastIndex));
};

const removeQuotedText = (formula) =>
  formula.replace(/"[^"\n]*"|'[^'\n]*'/g, '');

const validatePairs = (formula, openChar, closeChar, errorMessage) => {
  let count = 0;
  for (const char of formula) {
    if (char === openChar) count += 1;
    if (char === closeChar) count -= 1;
    if (count < 0) return errorMessage;
  }
  return count === 0 ? '' : errorMessage;
};

const validateFormula = (rawFormula) => {
  const formula = rawFormula.trim();
  if (!formula) return 'Vui lòng nhập công thức cần kiểm tra.';

  const normalizedFormula = formula.startsWith('=') ? formula.slice(1).trim() : formula;
  if (!normalizedFormula) return 'Công thức phải có biểu thức sau dấu bằng.';

  const formulaWithoutText = removeQuotedText(normalizedFormula);
  const invalidChar = formulaWithoutText.match(/[^A-Za-z0-9_\s()[\]+\-*\/^=,.;<>]/);
  if (invalidChar) return `Công thức chứa ký tự không hợp lệ: ${invalidChar[0]}.`;

  const bracketError = validatePairs(formulaWithoutText, '[', ']', 'Cặp ngoặc vuông của mã thành phần lương chưa hợp lệ.');
  if (bracketError) return bracketError;

  const parenthesisError = validatePairs(formulaWithoutText, '(', ')', 'Cặp ngoặc tròn trong công thức chưa hợp lệ.');
  if (parenthesisError) return parenthesisError;

  if (/[\+\-*\/^,;.]$/.test(formulaWithoutText.trim())) {
    return 'Công thức không được kết thúc bằng toán tử hoặc dấu phân tách.';
  }

  if (/^[*\/^,;.]/.test(formulaWithoutText.trim())) {
    return 'Công thức không được bắt đầu bằng toán tử hoặc dấu phân tách.';
  }

  if (/[\+\-*\/^]{2,}/.test(formulaWithoutText.replace(/\s+/g, ''))) {
    return 'Công thức có toán tử liên tiếp chưa hợp lệ.';
  }

  return '';
};

const checkLogic = () => {
  const errorMessage = validateFormula(formulaValue.value);
  localError.value = errorMessage;
  validationStatus.value = errorMessage ? 'error' : 'success';
  validationMessage.value = errorMessage ? '' : 'Công thức hợp lệ.';
  emit('validated', { valid: !errorMessage, message: errorMessage });

  toast.show({
    message: errorMessage || 'Công thức hợp lệ.',
    type: errorMessage ? 'error' : 'success',
  });
};
</script>

<style scoped>
.ms-formula {
  display: flex;
  flex-direction: column;
  width: 100%;
}

.ms-label {
  font-weight: 600;
  font-size: 13px;
  color: #111;
}

.editor-outer {
  border: 1px solid var(--border-color);
  background-color: #fff;
  overflow: hidden;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.editor-outer:focus-within {
  border-color: var(--color-primary);
  box-shadow: 0 0 4px rgba(44, 160, 28, 0.2);
}

.editor-container {
  height: 160px;
  background: #ffffff;
}

.my-editor {
  background: transparent;
  color: #111;
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
  font-size: 14px;
  line-height: 1.6;
  padding: 8px;
}

.formula-footer {
  border-top: 1px solid var(--border-color);
  background-color: #fff;
  font-size: 12px;
  gap: 12px;
}

.hint {
  color: #666;
  font-style: italic;
}

.btn-check-logic {
  background: none;
  border: none;
  color: #2ca01c;
  cursor: pointer;
  font-weight: 600;
  padding: 4px 8px;
  border-radius: 4px;
  transition: background-color 0.2s;
  white-space: nowrap;
}

.btn-check-logic:hover {
  background-color: #ebf5ea;
}

.ms-icon--check-green {
  background-color: #2ca01c;
}

.has-error .editor-outer {
  border-color: #eb5757;
}

.is-valid .editor-outer {
  border-color: #2ca01c;
}

.error-msg,
.formula-status {
  font-size: 12px;
  margin-top: 4px;
}

.error-msg,
.formula-status.is-error {
  color: #eb5757;
}

.formula-status.is-success {
  color: #2ca01c;
}

:deep(.prism-editor__line-number) {
  color: #bbb;
  padding-right: 12px !important;
}

:deep(.prism-editor__textarea:focus) {
  outline: none;
}

:deep(.prism-editor__container) {
  height: 100%;
}

:deep(.formula-token.number) {
  color: #111 !important;
}

:deep(.formula-token.string) {
  color: #007bff !important;
}

:deep(.formula-token.operator) {
  color: #eb5757 !important;
  font-weight: 600;
}

:deep(.formula-token.function) {
  color: #7b4dd6 !important;
  font-weight: 600;
}

:deep(.formula-token.component) {
  color: var(--text-formula, #005af1) !important;
  font-weight: 600;
}

:deep(.formula-token.identifier) {
  color: #555 !important;
}
</style>
