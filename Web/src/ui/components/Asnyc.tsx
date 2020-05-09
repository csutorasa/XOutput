import React, { Children, ReactNode, ReactFragment } from "react";
import CircularProgress from '@material-ui/core/CircularProgress';
import Typography from '@material-ui/core/Typography';
import { StyleGenerator, Styled } from "../../utils";
import { withStyles } from "@material-ui/core";


type ClassNames = 'centered';

const styles: StyleGenerator<ClassNames> = () => ({
  centered: {
      textAlign: 'center',
  },
});

export interface AsyncComponentProps<T> extends Styled<ClassNames> {
    task: Promise<T>;
    size?: number | string;
}

interface AsyncComponentState<T> {
    loaded: boolean;
    promise?: Promise<T>;
    success?: boolean;
    result?: T;
    error?: string;
}

class AsyncComponent<T> extends React.Component<AsyncComponentProps<T>, AsyncComponentState<T>> {

    state: AsyncComponentState<T> = {
        loaded: false,
    }

    componentDidMount() {
        if (this.props.task) {
            this.subscribe();
        }
    }

    componentDidUpdate() {
        if (this.props.task && (!this.state.promise || this.state.promise != this.props.task)) {
            this.subscribe();
        }
    }

    private subscribe(): void {
        this.setState({
            loaded: false,
            promise: this.props.task,
            success: null,
            result: null,
            error: null,
        })
        this.props.task.then((data) => {
            this.setState({
                loaded: true,
                success: true,
                result: data,
                error: null,
            })
        }, (err: Error) => {
            this.setState({
                loaded: true,
                success: false,
                result: null,
                error: err.toString(),
            })
            console.log('' + err)
        });
    }

    private handleChildren(node: React.ReactNode): ReactNode {
        if (node instanceof Array) {
            if (node.length === 1) {
                return this.handleChildren(node);
            }
            throw new Error('Only 1 function children is expected');
        } else 
        if (node instanceof Function) {
            return node(this.state.result);
        } else {
            throw new Error('Only 1 function children is expected');
        }
    }

    render() {
        const { classes } = this.props;
        if (this.state.loaded === false) {
            return <div className={classes.centered}><CircularProgress size={this.props.size || '10rem'} /></div>
        }
        if (this.state.success === false) {
            return <Typography color='error'>{this.state.error}</Typography>
        }
        return <>
            {this.handleChildren(this.props.children)}
        </>;
    }
}

export const Async = withStyles(styles)(AsyncComponent);
export default Async;
