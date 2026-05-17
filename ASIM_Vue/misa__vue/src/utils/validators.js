/**
 * Các quy tắc validate chuẩn Senior
 */
export const validators = {
  required: (val, label) => {
    if (!val || (typeof val === 'string' && val.trim() === '')) {
      return `${label} không được để trống.`;
    }
    return null;
  },
  maxLength: (val, max, label) => {
    if (val && val.length > max) {
      return `${label} không được vượt quá ${max} ký tự.`;
    }
    return null;
  },
  unique: (val, list, field, label) => {
    const isDuplicate = list.some(item => item[field] === val);
    if (isDuplicate) {
      return `${label} đã tồn tại trong hệ thống.`;
    }
    return null;
  }
};

export const validateForm = (data, rules) => {
  const errors = {};
  let isValid = true;

  for (const field in rules) {
    for (const rule of rules[field]) {
      const error = rule(data[field]);
      if (error) {
        errors[field] = error;
        isValid = false;
        break; // Chỉ lấy lỗi đầu tiên của mỗi trường
      }
    }
  }

  return { isValid, errors };
};
