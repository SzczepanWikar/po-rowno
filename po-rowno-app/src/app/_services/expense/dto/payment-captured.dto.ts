import { PaymentLinks } from './payment-links.dto';

export interface PaymentCapturedDto {
  id: string;
  status: string;
  links: PaymentLinks;
}
