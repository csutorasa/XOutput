import React, { ReactNode, Component, PropsWithChildren } from 'react';
import { Translation } from '../translation/Translation';

export type TextTranslatedTextProps = {
  text: string;
};

export type ChildrenTranslatedTextProps = {
  children: string;
};

export type TranslatedTextProps = TextTranslatedTextProps | ChildrenTranslatedTextProps;

function hasText(props: TranslatedTextProps): props is TextTranslatedTextProps {
  return !!(props as TextTranslatedTextProps).text;
}

export const TranslatedText = (props: TranslatedTextProps) => {
  let translationKey: string;
  if (hasText(props)) {
    translationKey = props.text;
  } else {
    translationKey = props.children;
  }
  const translatedText = Translation.translate(translationKey);
  return <>{translatedText}</>;
};

export const TT = TranslatedText;
export default TT;
