import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const password = control.value as string;

    if (!password) {
      return { required: 'Password cannot be null' };
    }

    const errors: ValidationErrors = {};

    if (password.length < 12) {
      errors['minLength'] = 'VALIDATION.PASSWORD_LENGTH';
    }

    if (!/[0-9]/.test(password)) {
      errors['hasNumber'] = 'VALIDATION.NO_NUMBER';
    }

    if (!/[A-Z]/.test(password)) {
      errors['hasUpperChar'] = 'VALIDATION.NO_UPPERCASE_CHAR';
    }

    if (!/[a-z]/.test(password)) {
      errors['hasLowerChar'] = 'VALIDATION.NO_LOWER_CHAR';
    }

    if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
      errors['hasSymbol'] = 'VALIDATION.NO_SPECIAL_CHARACTER';
    }

    return Object.keys(errors).length ? errors : null;
  };
}
