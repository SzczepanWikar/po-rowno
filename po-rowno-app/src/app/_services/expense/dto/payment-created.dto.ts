import { PaymentLinks } from './payment-links.dto';

export interface PaymentCreatedDto {
  id: string;
  orderId: string;
  links: PaymentLinks;
}
