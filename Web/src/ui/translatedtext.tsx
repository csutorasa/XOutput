import React, { ReactNode, Component } from "react";
import { Translation } from "../translation/translation";

export interface TranslatedTextProps {
    text?: string;
}

export class TranslatedText extends Component<TranslatedTextProps, any, any> {

    private childText(children: ReactNode): string {
        return children.toString();
    }

    render() {
        let text: string;
        if (this.props.text) {
            text = this.props.text;
        } else {
            text = this.childText(this.props.children);
        }
        const translatedText = Translation.translate(text);
        return <>{translatedText}</>;
    }
}

export const TT = TranslatedText;
export default TT;
